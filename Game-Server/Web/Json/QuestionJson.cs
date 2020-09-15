using Game_Server.Controller.Database.Tables;
using Game_Server.Util.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game_Server.Util.Json
{
    public class QuestionJson
    {
        [JsonProperty("question")]
        public string Question { get; set; }
        [JsonProperty("topicId")]
        public int TopicId { get; set; }
        [JsonProperty("correctAns")]
        public string Answer { get; set; }
        [JsonProperty("wrongAns")]
        public string[] Answers { get; set; }

        public Model.Question ToModel()
        {
            Model.Answer[] answers = new Model.Answer[Answers.Length + 1];
            answers[0] = new Model.Answer()
            {
                IsCorrect = true,
                Text = Answer
            };
            for(int i = 0; i < Answers.Length; i++)
            {
                answers[i + 1] = new Model.Answer()
                {
                    IsCorrect = false,
                    Text = Answers[i]
                };
            }
            Model.Question question = new Model.Question()
            {
                QuestionStr = Question,
                TopicId = this.TopicId,
                Answers = answers,
            };
            return question;
        }
    }
}
