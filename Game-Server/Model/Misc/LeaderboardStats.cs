using Game_Server.Controller.Database.Tables;
using Game_Server.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game_Server.Model
{
    public class LeaderboardStats : ISerializable
    {
        public int RequirementsCorrect { get; set; }
        public int RequirementsTotalQns { get; set; }
        public int DesignCorrect { get; set; }
        public int DesignTotalQns { get; set; }
        public int ImplementationCorrect { get; set; }

        public int ImplementationTotalQns { get; set; }
        public int TestingCorrect { get; set; }

        public int TestingTotalQns { get; set; }
        public int DeploymentCorrect { get; set; }
        public int DeploymentTotalQns { get; set; }
        public int MaintenanceCorrect { get; set; }
        public int MaintenanceTotalQns { get; set; }

        public List<QuestionAttempted> Attempted = new List<QuestionAttempted>();

        public LeaderboardStats(ICollection<QuestionAttempted> attempts)
        {
            this.Attempted = attempts.ToList();
        }

        public void Serialize(SerializeWriter writer)
        {
            var questionsAttempted = Attempted.GroupBy(qa => ServerMain.Instance.Database.GetQuestion(qa.QuestionId).TopicId).OrderBy(g => g.Key).ToList();

            for(int i = 1; i <= 6; i++)
            {
                var q = questionsAttempted.SingleOrDefault(grp => grp.Key == i);
                if (q == null)
                {
                    writer.Write(0);
                    writer.Write(0);
                }
                else
                {
                    var answersCorrectly = q.Where(a => ServerMain.Instance.Database.GetAnswer(a.AnswerId).IsCorrect == 1).Count();
                    var answers = q.Count();
                    writer.Write(answersCorrectly);
                    writer.Write(answers);
                }
            }
        }
    }
}
