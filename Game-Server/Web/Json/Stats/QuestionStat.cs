using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game_Server.Web.Json
{
    public class QuestionStat
    {
        [JsonProperty("questionId")]
        public int QuestionId { get; set; }

        [JsonProperty("questionText")]
        public string QuestionText { get; set; }

        [JsonProperty("correctness")]
        public float Percentage 
        {
            get { return (1f*CorrectAnswerCount) / (WrongAnswerOneCount + WrongAnswerTwoCount + WrongAnswerThreeCount + CorrectAnswerCount) * 100; }
        }

        [JsonProperty("correctAnsStudentsCount")]
        public int CorrectAnswerCount { get; set; }

        [JsonProperty("correctAnsText")]
        public string CorrectAnswer { get; set; }

        [JsonProperty("wrongAns1StudentsCount")]
        public int WrongAnswerOneCount { get; set; }

        [JsonProperty("wrongAns1Text")]
        public string WrongAnswerOne { get; set; }

        [JsonProperty("wrongAns2StudentsCount")]
        public int WrongAnswerTwoCount { get; set; }
        [JsonProperty("wrongAns2Text")]
        public string WrongAnswerTwo { get; set; }

        [JsonProperty("wrongAns3StudentsCount")]
        public int WrongAnswerThreeCount { get; set; }
        [JsonProperty("wrongAns3Text")]
        public string WrongAnswerThree { get; set; }
    }
}
