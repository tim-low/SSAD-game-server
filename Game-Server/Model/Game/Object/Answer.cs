using System;
using Game_Server.Util;
using Game_Server.Util.Database;
using Newtonsoft.Json;

namespace Game_Server.Model
{
    public class Answer : ISerializable, IDatabase<Controller.Database.Tables.Answer>
    {
        public int Id;
        [JsonProperty("Text")]
        public string Text;
        public bool IsCorrect = false;

        public void FromEntity(Controller.Database.Tables.Answer item)
        {
            this.Id = item.Id;
            this.Text = item.Text;
            this.IsCorrect = item.IsCorrect == 1;
        }

        public void Serialize(SerializeWriter writer)
        {
            writer.Write(Id);
            writer.WriteText(Text, false);
        }

        public Controller.Database.Tables.Answer ToEntity()
        {
            return new Controller.Database.Tables.Answer()
            {
                IsCorrect = IsCorrect ? 1 : 0,
                Text = this.Text
            };
        }
    }
}