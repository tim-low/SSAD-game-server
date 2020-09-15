using Game_Server.Controller.Database.Tables;
using Game_Server.Util.Database.Tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game_Server.Database
{
    /// <summary>
    ///  Entity Framework Core for Database connection to increase the maintainability of server code
    /// </summary>
    internal class SEContext : DbContext
    {
        internal SEContext(DbContextOptions<SEContext> options)
        : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Account Entity Rule
            modelBuilder.Entity<Account>().HasOne<Character>(s => s.Character)
                .WithOne(c => c.Account).HasForeignKey<Character>(c => c.AccountId);
            modelBuilder.Entity<Inventory>().HasOne<Account>(i => i.Account)
                .WithOne(a => a.Inventory).HasForeignKey<Inventory>(i => i.AccountId);
 

            // Score Entity Rule
            modelBuilder.Entity<Score>().HasOne<Account>(s => s.CreatedBy)
                .WithMany(a => a.Scores).HasForeignKey(s => s.AccountId);
            modelBuilder.Entity<Score>().HasOne<Topic>(s => s.Topic).WithMany(t => t.Scores).HasForeignKey(s => s.TopicId);

            // Mastery Entity Rule
            modelBuilder.Entity<Mastery>().HasKey(e => new { e.AccountId, e.TopicId });
            modelBuilder.Entity<Mastery>().HasOne<Topic>(m => m.Topic).WithMany(t => t.Masteries).HasForeignKey(m => m.TopicId);
            modelBuilder.Entity<Mastery>().HasOne<Account>(m => m.Account)
                .WithMany(a => a.Masteries).HasForeignKey(m => m.AccountId);

            modelBuilder.Entity<Session>().HasKey(e => new { e.Id, e.AccountId });
            modelBuilder.Entity<Session>().HasOne<Account>(s => s.Account).WithMany(a => a.Sessions).HasForeignKey(s => s.AccountId);
            modelBuilder.Entity<Session>().HasOne<Quiz>(s => s.Quiz).WithMany(q => q.Sessions).HasForeignKey(s => s.QuizId);

            // Topic Entity Rule
            modelBuilder.Entity<Topic>().HasMany(t => t.Masteries);
            modelBuilder.Entity<Topic>().HasMany(t => t.Questions);
            modelBuilder.Entity<Topic>().HasMany(t => t.Scores);

            modelBuilder.Entity<SessionQuestion>().HasKey(e => new { e.SessionId, e.QuestionId });
            modelBuilder.Entity<SessionQuestion>().HasOne<Question>(s => s.Question);

            modelBuilder.Entity<Question>().HasOne<Topic>(q => q.CategorizedBy)
                .WithMany(t => t.Questions);

            modelBuilder.Entity<QuestionAttempted>().HasOne<Question>(qa => qa.Question).WithMany(q => q.Questions).HasForeignKey(qa => qa.QuestionId);
            modelBuilder.Entity<QuestionAttempted>().HasOne<Answer>(qa => qa.Answer).WithMany(a => a.Attempteds).HasForeignKey(qa => new { qa.AnswerId });
            modelBuilder.Entity<QuestionAttempted>().HasOne<Account>(qa => qa.Account).WithMany(a => a.QuestionAttempteds).HasForeignKey(qa => qa.AttemptedBy);

            modelBuilder.Entity<QuestionQuiz>().HasKey(e => new { e.QuestionId, e.QuizId });
            modelBuilder.Entity<QuestionQuiz>().HasOne<Question>(qq => qq.Question).WithMany(q => q.Quizzes).HasForeignKey(qq => qq.QuestionId);
            modelBuilder.Entity<QuestionQuiz>().HasOne<Quiz>(qq => qq.Quiz).WithMany(q => q.Questions).HasForeignKey(qq => qq.QuizId);

            modelBuilder.Entity<Answer>().HasKey(e => new { e.Id });
            modelBuilder.Entity<Answer>().HasOne<Question>(a => a.AnswerFor)
                .WithMany(q => q.Answers);

            modelBuilder.Entity<Gloryboard>().HasKey(e => e.Id);
            modelBuilder.Entity<Gloryboard>().HasOne<Account>(g => g.Account);

            modelBuilder.Entity<AnnoucementRecord>().HasKey(a => a.Id);
            modelBuilder.Entity<AnnoucementRecord>().HasMany(a => a.Events);

            modelBuilder.Entity<EventRecord>().HasKey(e => e.Id);
            modelBuilder.Entity<EventRecord>().HasOne<AnnoucementRecord>(e => e.Annoucement);
            /**
            modelBuilder.Entity<Character>()
                .HasOne<Account>(c => c.Account)
                .WithOne(a => a.Character)
                .HasForeignKey<Character>(c => c.AccountId);
    */
        }
    }
}
