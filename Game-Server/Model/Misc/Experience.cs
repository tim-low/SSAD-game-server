using Game_Server.Controller.Database.Tables;
using Game_Server.Util;
using Game_Server.Util.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game_Server.Model
{
    public class Experience : ISerializable
    {
        public string Id { get; set; }
        public List<Mastery> Masteries {get; private set;}

        public Experience(ICollection<Mastery> masteries)
        {
            this.Masteries = masteries.OrderBy(mastery => mastery.TopicId).ToList();
        }

        public void Serialize(SerializeWriter writer)
        {
            foreach(Mastery mastery in Masteries)
            {
                writer.Write(mastery.Exp);
            }
        }

        public Mastery GetMasteryByTopic(int topicId)
        {
            return this.Masteries.SingleOrDefault(t => t.TopicId == topicId);
        }

    }
}
