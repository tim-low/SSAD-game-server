using System.Collections.Generic;
using Game_Server.Util;
using Game_Server.Util.Database;

namespace Game_Server.Model
{
    public class Quiz : ISerializable, IDatabase<Controller.Database.Tables.Quiz>
    {
        public int Id;
        public string Name;

        public List<Question> Questions;

        public Quiz()
        {
            Questions = new List<Question>();
        }

        public void Serialize(SerializeWriter writer)
        {
            writer.Write(Id);
            writer.Write(Name);
            writer.Write(Questions.Count);
            foreach(Question question in Questions)
            {
                writer.Write(question);
            }
        }

        public void FromEntity(Controller.Database.Tables.Quiz item)
        {
            Log.Debug("Invoke FromEntity()");
            this.Id = item.Id;
            Log.Debug("{0}", item.Name);
            this.Name = item.Name;
            this.Questions = new List<Question>();
            foreach(var question in item.Questions)
            {
                Question model = new Question();
                model.FromEntity(question.Question);
                Questions.Add(model);
            }
        }

        public Controller.Database.Tables.Quiz ToEntity()
        {
            throw new System.NotImplementedException();
        }
    }
}
