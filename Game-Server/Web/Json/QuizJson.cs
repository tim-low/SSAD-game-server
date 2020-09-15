﻿using Game_Server.Controller.Database.Tables;
using Game_Server.Util.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game_Server.Util.Json
{
    /// <summary>
    /// Need a customize Json Response because the Json for Quiz is different
    /// from our Database Quiz Model
    /// </summary>
    public class QuizJson : IDatabase<Quiz>
    {
        /// <summary>
        /// This is the token generated by the server to the client
        /// </summary>
        [JsonProperty("username")]
        public string CreatedBy { get; set; }

        [JsonProperty("quizName")]
        public string QuizName { get; set; }

        [JsonProperty("questionIds")]
        public int[] QuestionId { get; set; }

        public void FromEntity(Quiz item)
        {
            throw new NotImplementedException("Not Needed");
        }

        public Quiz ToEntity()
        {
            Quiz quiz = new Quiz();
            quiz.IsCustom = 1;
            quiz.Name = QuizName;
            return quiz;
        }
    }
}