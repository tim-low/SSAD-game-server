using Game_Server.Util;
using Game_Server.Util.Database;
using Game_Server.Util.Database.Tables;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game_Server.Model.Misc
{
    public class Event : IDatabase<EventRecord>
    {
        public int Id { get; set; }
        public EventType Type { get; set; }
        public int Value { get; set; }

        public void FromEntity(EventRecord item)
        {
            this.Id = item.Id;
            this.Type = (EventType)item.EventType;
            this.Value = item.Value;
        }

        public EventRecord ToEntity()
        {
            EventRecord record = new EventRecord()
            {
                Id = this.Id,
                EventType = (int) this.Type,
                Value = this.Value
            };
            return record;
        }
    }

    public enum EventType
    {
        ExpRate,
        LootRate,
        BaseExp
    }
}
