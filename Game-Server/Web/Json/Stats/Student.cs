using Game_Server.Controller.Database.Tables;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game_Server.Web.Json
{
    public class Student
    {
        [JsonProperty("studentName")]
        public string Name { get; set; }

        [JsonProperty("classGroup")]
        public string ClassGroup { get; set; }

        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("answerPicked")]
        public int[] Answers
        {
            get { return QuestionsAnswers.Values.ToArray(); }
        }

        [JsonProperty("numCorrect")]
        public int NumOfCorrect { get; set; }

        [JsonProperty("correctness")]
        public float Percentage 
        {
            get { return (1f*NumOfCorrect / QuestionsAnswers.Values.Count()) * 100; }
        }

        /// <summary>
        /// This field will be ignore will output as json file
        /// </summary>
        [JsonIgnore]
        public SortedDictionary<int, int> QuestionsAnswers { get; set; }

        public Student()
        {
            QuestionsAnswers = new SortedDictionary<int, int>();
        }

        public Student(List<int> Question, Account account)
        {
            QuestionsAnswers = new SortedDictionary<int, int>();
            foreach (int i in Question)
            {
                QuestionsAnswers[i] = 0;
            }
            this.Name = account.FullName;
            this.Year = account.Year;
            this.ClassGroup = account.Class;
        }
    }
}
