using Game_Server.Controller.Database.Tables;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game_Server.Web.Json
{
    public class StudentStat
    {

        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("classGroup")]
        public string ClassGroup { get; set; }

        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("semester")]
        public int Semester { get; set; }

        public StudentStat(string name, string classGrp, int year, int semester)
        {
            this.Name = name;
            this.ClassGroup = classGrp;
            this.Year = year;
            this.Semester = semester;
        }


        [JsonProperty("totalAttempts")]
        public int TotalAttempts
        {
            get { return Attempts.Count(); }
        }

        [JsonProperty("totalCorrect")]
        public int TotalCorrect
        {
            get { return Attempts.Where(attempt => attempt.Answer.IsCorrect == 1).Count(); }
        }

        [JsonProperty("correctness")]
        public double Correctness
        {
            get 
            {
                if (TotalAttempts == 0)
                    return 0;
                return (1.0f * TotalCorrect / TotalAttempts) * 100; 
            }
        }

        /// <summary>
        /// Initialize it to prevent error
        /// </summary>
        [JsonIgnore]
        public List<QuestionAttempted> Attempts = new List<QuestionAttempted>();
    }
}
