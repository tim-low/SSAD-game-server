using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Game_Server.Controller.Database;
using Game_Server.Controller.Database.Tables;
using Game_Server.Model;
using Game_Server.Model.Misc;
using Game_Server.Util;
using Game_Server.Util.Database;
using Game_Server.Util.Database.Tables;
using Game_Server.Util.Json;
using Game_Server.Web.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using MySql.Data.MySqlClient;

namespace Game_Server.Database
{
    public class GameDatabase
    {
        /// <summary>
        /// TODO: concurrency issue need to be fix when there is a conflict in values of the same properties.
        /// </summary>
        private string _connectionString;
        private SEContext Database;
        private Timer PeriodicallySaveTimer;
        private Mutex Mutex;

        /// <summary>
        /// Storing of local copy Tables which can be used to retrieve data
        /// </summary>
        private Hashtable Tables;

        public bool IsConnected
        {
            get
            {
                try
                {
                    Database.Database.OpenConnection();
                    Database.Database.CloseConnection();
                }
                catch (Exception e)
                {
                    Log.Debug(e.StackTrace);
                    return false;
                }
                return true;
            }
        }
        /// <summary>
        ///     Sets connection string and calls TestConnection.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="db"></param>
        public void Init(string host, int port, string user, string pass, string db)
        {
            _connectionString =
                $"server={host}; port={port}; database={db}; uid={user}; password={pass}; pooling=true; min pool size=0; max pool size=100; ConvertZeroDateTime=true";
            var optionsBuilder = new DbContextOptionsBuilder<SEContext>();
            optionsBuilder.UseMySQL(_connectionString);
            optionsBuilder.EnableSensitiveDataLogging(true);
            Database = new SEContext(optionsBuilder.Options);
            Tables = new Hashtable();
            Mutex = new Mutex(false, "Database");
            Log.Info("Starting process to backup database.");
            PeriodicallySaveTimer = new Timer(SaveDatabase, Database, Timeout.Infinite, 1000*60*15);
        }

        private async void SaveDatabase(object state)
        {
            try
            {
                Mutex.WaitOne();
                await Database.SaveChangesAsync();
                Mutex.ReleaseMutex();
                Log.Info("Database saved.");
            }
            catch (InvalidOperationException e)
            {
                Log.Error("Failed to save database");
            }
        }

        /// <summary>
        /// Storing the Entity Table in a memory location first, to ensure that we can query and have the latest update made before flushing
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public DbSet<T> GetDbSet<T>() where T : class
        {
            DbSet<T> db = this.Database.Set<T>();
            return db;
        }

        #region ACCOUNT OPERATION
        private DbSet<Account> Accounts()
        {
            return this.GetDbSet<Account>();
        }

        public void ForcedLogout()
        {
            Mutex.WaitOne();
            foreach(var account in Accounts())
            {
                account.IsLoggedIn = 0;
            }
            Database.SaveChanges();
            Mutex.ReleaseMutex();
        }

        public Controller.Database.Tables.Character GetCharacter(string username)
        {
            Mutex.WaitOne();
            var @char = this.GetDbSet<Controller.Database.Tables.Character>()
                .Include(character => character.Account)
                    .ThenInclude(account => account.QuestionAttempteds)
                .Include(character => character.Account)
                    .ThenInclude(account => account.Masteries)
                .Include(character => character.Account)
                    .ThenInclude(account => account.Scores)
                .Include(character => character.Account)
                    .ThenInclude(account => account.Sessions)
                .Include(character => character.Account)
                    .ThenInclude(account => account.Inventory)
                .Where(character => character.Account.Username == username).FirstOrDefault();
            Mutex.ReleaseMutex();
            return @char;
        }

        public int CheckIfAccountExist(string username, string email)
        {
            if(Accounts().Where(a => a.Username == username).Count() > 0)
            {
                return 1;
            }
            else if(Accounts().Where(a => a.Email == email).Count() > 0)
            {
                return 2;
            }
            else
            {
                return 0;
            }
        }

        public void CreateAccount(string username, string email, string password, string fullname, string @class, int year, int semester, string salt)
        {
            Account account = new Account()
            {
                Id = Utilities.GenerateGuid().ToString(),
                Username = username,
                Email = email,
                FullName = fullname,
                Class = @class,
                Year = year,
                Semester = semester,
                Password = Password.GenerateSaltedHash(password, salt),
                Salt = salt,
                Masteries = new List<Mastery>(),
                QuestionAttempteds = new List<QuestionAttempted>(),
                Sessions = new List<Session>(),
                Scores = new List<Score>(),
                CreationDate = DateTime.Now,
                LastLoggedIn = DateTime.Now,
                IsLoggedIn = 1
            };
            account.Character = new Controller.Database.Tables.Character()
            {
                Id = Utilities.GenerateGuid().ToString(),
                TopEqp = 8,
                HeadEqp = 8,
                BottomEqp = 8,
                ShoeEqp = 8,
                ChestCount = 0,
                AccountId = account.Id
            };
            for(int i = 0; i < this.GetDbSet<Topic>().Where(topic => topic.Id != 7).Count(); i++)
            {
                Mastery mastery = new Mastery()
                {
                    AccountId = account.Id,
                    Exp = 0,
                    TopicId = (i + 1)
                };
                account.Masteries.Add(mastery);
            }
            Inventory inventory = new Inventory()
            {
                Head = 0,
                Shirt = 0,
                AccountId = account.Id,
                Pant = 0,
                Shoe = 0
            };
            account.Inventory = inventory;
            Mutex.WaitOne();
            Accounts().Add(account);
            Database.SaveChanges();
            Mutex.ReleaseMutex();
        }
        #endregion

        #region Leaderboard
        public List<Model.Ranking> GetLeaderboard(string username, int lifeCycleStages, int pageNum, int pageSize, out int ownRank, out bool isLastPage)
        {
            ownRank = 0;
            List<Model.Ranking> allrankings;
            Mutex.WaitOne();
            if (lifeCycleStages == 6)
            {
                var query = from account in GetDbSet<Account>().AsNoTracking()
                            join score in GetDbSet<Score>().AsNoTracking()
                            on account.Id equals score.AccountId into account_score
                            select new Ranking() { Username = account.Username, Score = account_score.Sum(s => s.Point) };
                allrankings = query.OrderByDescending(r => r.Score).ToList();
            }
            else
            {
                var query = from account in GetDbSet<Account>().AsNoTracking()
                            join score in GetDbSet<Score>().Where(s => s.TopicId == lifeCycleStages + 1).AsNoTracking()
                            on account.Id equals score.AccountId into account_score
                            select new Ranking() { Username = account.Username, Score = account_score.Sum(s => s.Point) };
                allrankings = query.OrderByDescending(r => r.Score).ToList();
            }
            Mutex.ReleaseMutex();
            /*

            if (lifeCycleStages == 6)
            {
                allrankings = this.GetDbSet<Score>().Include(s => s.CreatedBy).GroupBy(s => s.AccountId)
                    .Select(r => new Model.Ranking()
                    {
                        Username = r.Select(i => i.CreatedBy.Username).FirstOrDefault(),
                        Score = r.Sum(i => i.Point),
                    }).OrderByDescending(a => a.Score).ToList();
            }
            else
            {
                allrankings = this.GetDbSet<Score>().Include(s => s.CreatedBy).Where(s => s.TopicId == lifeCycleStages+1).GroupBy(s => s.AccountId)
                    .Select(r => new Model.Ranking()
                    {
                        Username = r.Select(i => i.CreatedBy.Username).FirstOrDefault(),
                        Score = r.Sum(i => i.Point),
                    }).OrderByDescending(a => a.Score).ToList();
            }
            */

            isLastPage = allrankings.Count() <= ((pageNum+1) * pageSize);

            ownRank = allrankings.SingleOrDefault(r => r.Username == username) == null ? -1 : allrankings.IndexOf(allrankings.Single(r => r.Username == username)) + 1;

            return allrankings.Skip((pageNum) * pageSize).Take(pageSize).ToList();
        }

        public List<Gloryboard> GetGloryboard(int pageNum, int pageSize, out bool isLastPage)
        {
            Mutex.WaitOne();
            List<Gloryboard> gloryboards = this.GetDbSet<Gloryboard>().Include(g => g.Account).AsNoTracking().OrderBy(g => g.CompletionDate).ToList();
            Mutex.ReleaseMutex();
            isLastPage = gloryboards.Count() <= (pageNum+1) * pageSize;
            return gloryboards.Skip((pageNum - 1) * pageSize).Take(pageSize).ToList();
        }
        #endregion

        public ICollection<Model.Question> GetQuestions(int TopicId)
        {
            List<Model.Question> questionBank = new List<Model.Question>();
            Mutex.WaitOne();
            var x = this.GetDbSet<Controller.Database.Tables.Question>().Include(q => q.Answers).Where(q => q.TopicId == TopicId && q.IsCustom == 0);
            Mutex.ReleaseMutex();
            foreach(Controller.Database.Tables.Question question in x)
            {
                Model.Question q = new Model.Question();
                q.FromEntity(question);
                questionBank.Add(q);
            }
            return questionBank;
        }

        public Controller.Database.Tables.Quiz GetQuiz(int id)
        {
            Mutex.WaitOne();
            var quiz = this.GetDbSet<Controller.Database.Tables.Quiz>()
                .Include(quiz => quiz.Questions)
                    .ThenInclude(quizQuestion => quizQuestion.Question)
                        .ThenInclude(question => question.Answers)
                .Where(quiz => quiz.Id == id).FirstOrDefault();
            Mutex.ReleaseMutex();
            return quiz;
        }

        public Controller.Database.Tables.Question GetQuestion(int questionId)
        {
            Mutex.WaitOne();
            var question = this.GetDbSet<Controller.Database.Tables.Question>().Where(q => q.Id == questionId).First();
            Mutex.ReleaseMutex();
            return question;
        }

        public Controller.Database.Tables.Answer GetAnswer(int AnswerId)
        {
            Mutex.WaitOne();
            var answer = this.GetDbSet<Controller.Database.Tables.Answer>().Where(a => a.Id == AnswerId).First();
            Mutex.ReleaseMutex();
            return answer;
        }


        #region Teacher App
        public Controller.Database.Tables.Question AddTeacherQuestion(QuestionJson jsonObj, string uid)
        {
            var question = jsonObj.ToModel();
            question.CreatedBy = uid;
            var questionDb = question.ToEntity();
            questionDb.IsCustom = 1;
            questionDb.Difficulty = 1;
            Mutex.WaitOne();
            this.GetDbSet<Controller.Database.Tables.Question>().Add(questionDb);
            Database.SaveChanges();
            Mutex.ReleaseMutex();
            return questionDb;
        }

        /// <summary>
        /// This method return all questions that are either generic questions or questions created by the teacher themselves.
        /// </summary>
        /// <param name="uid">id of the teacher</param>
        /// <returns></returns>
        public ICollection<Controller.Database.Tables.Question> GetTeacherQuestions(string uid)
        {
            Mutex.WaitOne();
            ICollection <Controller.Database.Tables.Question> questions = this.GetDbSet<Controller.Database.Tables.Question>().Include(q => q.Answers).AsNoTracking().Where(q => q.IsCustom == 0 || (q.IsCustom == 1 && q.CreatedBy == uid)).ToList();
            Mutex.ReleaseMutex();
            return questions;
        }

        public Controller.Database.Tables.Quiz AddTeacherQuizzes(QuizJson jsonObj, string uid)
        {
            var newQuiz = jsonObj.ToEntity();
            newQuiz.CreatedBy = uid;
            newQuiz.Questions = new List<QuestionQuiz>();
            foreach(var questionId in jsonObj.QuestionId)
            {
                newQuiz.Questions.Add(new QuestionQuiz()
                {
                    QuestionId = questionId
                });
            }
            Mutex.WaitOne();
            ServerMain.Instance.Database.GetDbSet<Controller.Database.Tables.Quiz>().Add(newQuiz);
            Database.SaveChanges();
            Mutex.ReleaseMutex();
            return newQuiz;
        }

        public ICollection<Controller.Database.Tables.Quiz> GetTeacherQuizzes(string uid)
        {
            Mutex.WaitOne();
            ICollection<Controller.Database.Tables.Quiz> quiz = ServerMain.Instance.Database.GetDbSet<Controller.Database.Tables.Quiz>().Include(q => q.Questions).AsNoTracking().Where(q => q.CreatedBy == uid && q.IsCustom == 1).ToList();
            Mutex.ReleaseMutex();
            return quiz;
        }

        public void SaveSessionQuestion(List<SessionQuestion> questions)
        {
            Mutex.WaitOne();
            ServerMain.Instance.Database.GetDbSet<SessionQuestion>().AddRange(questions);
            Mutex.ReleaseMutex();
        }
        #endregion

        #region Analytic Features
        /// <summary>
        /// This is for Endpoint 3 as discussed on Discord
        /// </summary>
        /// <returns></returns>
        public QuizStat[] GetQuizStats()
        {
            Mutex.WaitOne();
            List<Controller.Database.Tables.Quiz> quizzes = GetDbSet<Controller.Database.Tables.Quiz>()
                .Include(quiz => quiz.Questions)
                    .ThenInclude(questionQuiz => questionQuiz.Question)
                        .ThenInclude(question => question.Questions)
                            .ThenInclude(questionAttempt => questionAttempt.Account)
                .Include(quiz => quiz.Questions)
                    .ThenInclude(questionQuiz => questionQuiz.Question)
                        .ThenInclude(question => question.Answers)
                .Include(quiz => quiz.Sessions)
                    .ThenInclude(session => session.Account)
                .Where(quiz => quiz.IsCustom == 1 && quiz.Sessions.Count > 0).ToList();
            // Process the data now
            QuizStat[] quizStats = new QuizStat[quizzes.Count];
            for(int i = 0; i < quizStats.Length; i++)
            {
                Controller.Database.Tables.Quiz quiz = quizzes.ElementAt(i);
                Dictionary<string, Student> StudentStat = new Dictionary<string, Student>();
                List<int> questionList = quiz.Questions.Select(quizQuestion => quizQuestion.QuestionId).ToList();
                foreach (Session session in quiz.Sessions)
                {
                    if (!StudentStat.ContainsKey(session.AccountId))
                        StudentStat.Add(session.AccountId, new Student(questionList, session.Account));
                }
                List<QuestionStat> questionStats = new List<QuestionStat>();
                List<string> sessions = quiz.Sessions.Select(x => x.Id).ToList();
                var quizStat = new QuizStat();
                quizStat.QuizName = quiz.Name;
                int questionCount = 0;
                foreach(QuestionQuiz questionQuiz in quiz.Questions)
                {
                    QuestionStat questionStat = new QuestionStat();
                    List<QuestionAttempted> allAttempt = new List<QuestionAttempted>();
                    Controller.Database.Tables.Question questionEntity = questionQuiz.Question;
                    questionStat.QuestionId = ++questionCount;
                    questionStat.QuestionText = questionEntity.Text;
                    var questionAttemptedForQuiz = questionEntity.Questions.Where(x => sessions.Contains(x.AttemptedFor)).ToList();
                    var individualAttempts = questionAttemptedForQuiz.GroupBy(x => x.AttemptedBy);
                    foreach(var studentAttempt in individualAttempts)
                    {
                        QuestionAttempted qa = studentAttempt.Last();
                        allAttempt.Add(qa);
                        StudentStat[qa.Account.Id].QuestionsAnswers[qa.QuestionId] = qa.AnswerId;
                        if(qa.Answer.IsCorrect == 1)
                        {
                            StudentStat[qa.Account.Id].NumOfCorrect += 1;
                        }
                    }
                    questionStat.CorrectAnswerCount = allAttempt.Where(attempt => attempt.Answer.IsCorrect == 1).Count();
                    questionStat.CorrectAnswer = questionEntity.Answers.Where(answer => answer.IsCorrect == 1).First().Text;
                    List<Controller.Database.Tables.Answer> wrongAnswers = questionEntity.Answers.Where(answer => answer.IsCorrect == 0).OrderBy(answer => answer.Id).ToList();

                    questionStat.WrongAnswerOneCount = allAttempt.Where(attempt => attempt.AnswerId == wrongAnswers.ElementAt(0).Id).Count();
                    questionStat.WrongAnswerOne = wrongAnswers.ElementAt(0).Text;
                    questionStat.WrongAnswerTwoCount = allAttempt.Where(attempt => attempt.AnswerId == wrongAnswers.ElementAt(1).Id).Count();
                    questionStat.WrongAnswerTwo = wrongAnswers.ElementAt(1).Text;
                    questionStat.WrongAnswerThreeCount = allAttempt.Where(attempt => attempt.AnswerId == wrongAnswers.ElementAt(2).Id).Count();
                    questionStat.WrongAnswerThree = wrongAnswers.ElementAt(2).Text;
                    questionStats.Add(questionStat);
                }
                quizStat.QuestionStats = questionStats.ToArray();
                quizStat.StudentAttempted = StudentStat.Values.ToArray();
                quizStats[i] = quizStat;
            }
            Mutex.ReleaseMutex();
            return quizStats;
        }

        public TopicMasteryStat[] GetTopicMasteryStats()
        {
            Mutex.WaitOne();
            TopicMasteryStat[] stats = new TopicMasteryStat[6];
            var students = GetDbSet<Account>().Where(account => account.Permission == 0);
            for (int i = 0; i < 6; i++)
            {
                stats[i] = new TopicMasteryStat();
                stats[i].TopicId = (i + 1);
                foreach (var student in students)
                {
                    StudentStat stat = new StudentStat(student.FullName, student.Class, student.Year, student.Semester);
                    stats[i].StudentStatsKeyValuePair[student.Id] = stat;
                }
            }
            var attempt  = GetDbSet<QuestionAttempted>()
                .Include(attempt => attempt.Account)
                .Include(attempt => attempt.Question)
                    .ThenInclude(question => question.Answers)
                .Include(attempt => attempt.Answer).Where(attempt => attempt.Question.IsCustom != 1).GroupBy(attempt => attempt.Question.TopicId);
            Mutex.ReleaseMutex();
            foreach (var topicAttempt in attempt) // get the attempt of each topic
            {
                int topicId = topicAttempt.Key;
                // Create a list of student in the system identified by their Id
                // MEDIAN
                // obtain correctness % of each student
                var studentsCorrectness = topicAttempt.GroupBy(item => item.Account.Id);
                foreach (var student in studentsCorrectness)
                {
                    if (stats[topicId - 1].StudentStatsKeyValuePair.ContainsKey(student.Key))
                    {
                        stats[topicId - 1].StudentStatsKeyValuePair[student.Key].Attempts.AddRange(student.ToList());
                    }
                }
            }
            return stats;
        }
        #endregion

        /// <summary>
        /// Call this method whenever you have a Entity.Add state to
        /// flush all changes to database immediately, as we are using
        /// a multitude of foreign key which is based on primary key and
        /// it's auto increment functionalities.
        /// </summary>
        public void Save()
        {
            Mutex.WaitOne();
            Database.SaveChanges();
            Mutex.ReleaseMutex();
        }

        #region Annoucement
        public async Task<List<AnnoucementRecord>> LoadAnnoucement()
        {
            return await GetDbSet<AnnoucementRecord>().Include(a => a.Events).ToListAsync();
        }
        #endregion
    }
}
