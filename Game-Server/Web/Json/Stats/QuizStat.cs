using Game_Server.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game_Server.Web.Json
{
    public class QuizStat
    {
        [JsonProperty("quizName")]
        public string QuizName { get; set; }

        [JsonProperty("highestScore")]
        public int HighestScore 
        {
            get
            {
                return StudentAttempted.Select(x => x.NumOfCorrect).Max();
            }
        }

        [JsonProperty("lowestScore")]
        public int LowestScore
        {
            get
            {
                return StudentAttempted.Select(x => x.NumOfCorrect).Min();
            }
        }

        [JsonProperty("mean")]
        public double Mean 
        {
            get
            {
                return StudentAttempted.Select(x => x.NumOfCorrect).Average();
            }
        }

        [JsonProperty("median")]
        public double Median 
        {
            get
            {
                var count = StudentAttempted.Length;
                int half = count / 2;
                var sorted = StudentAttempted.Select(x => x.NumOfCorrect).OrderBy(n => n);
                if ((count % 2) == 0)
                {
                    return (sorted.ElementAt(half) + sorted.ElementAt(half - 1)) / 2;
                }
                else
                {
                    return sorted.ElementAt(half);
                }
            }
        }

        [JsonProperty("standardDeviation")]
        public double StandardDeviation 
        {
            get
            {
                return StudentAttempted.Select(x => x.NumOfCorrect).StdDev();
            }
        }

        [JsonProperty("questionStats")]
        public QuestionStat[] QuestionStats { get; set; }

        [JsonProperty("studentsAttempted")]
        public Student[] StudentAttempted { get; set; }

    }
}
