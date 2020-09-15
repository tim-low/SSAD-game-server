using System;
using System.Collections.Generic;
using Game_Server.Util;
using Game_Server.Util.Database;
using Newtonsoft.Json;

namespace Game_Server.Model
{
    public class Question : ISerializable, IDatabase<Controller.Database.Tables.Question>
    {
        public int Id;
        [JsonProperty("question")]
        public string QuestionStr;
        [JsonProperty("topicId")]
        public int TopicId;
        [JsonProperty("answers")]
        public Answer[] Answers;
        public string CreatedBy { get; set; }

        public Question()
        {
            Answers = new Answer[4];
        }

        public void FromEntity(Controller.Database.Tables.Question item)
        {
            this.Id = item.Id;
            this.QuestionStr = item.Text;
            this.TopicId = item.TopicId;
            this.Answers = new Answer[item.Answers.Count];
            this.CreatedBy = item.CreatedBy;
            int i = 0;
            foreach(var answer in item.Answers)
            {
                this.Answers[i] = new Answer();
                this.Answers[i].FromEntity(answer);
                i++;
            }
        }

        public void Serialize(SerializeWriter writer)
        {
            writer.WriteText(QuestionStr, false);
            List<Answer> answers = new List<Answer>(Answers);
            answers.Shuffle();
            foreach(Answer answer in answers)
            {
                writer.Write(answer);
            }
        }

        public Controller.Database.Tables.Question ToEntity()
        {
            var answers = new List<Controller.Database.Tables.Answer>();
            foreach(var answer in this.Answers)
            {
                answers.Add(answer.ToEntity());
            }
            return new Controller.Database.Tables.Question()
            {
                Text = this.QuestionStr,
                TopicId = this.TopicId,
                Answers = answers,
                CreatedBy = this.CreatedBy
            };
        }
    }
}
