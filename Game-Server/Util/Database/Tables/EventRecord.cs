using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Game_Server.Util.Database.Tables
{
    [Table("events")]
    public class EventRecord
    {
        [Key]
        [Column("event_id")]
        public int Id { get; set; }

        [Column("event_type")]
        public int EventType { get; set; }

        [Column("event_value")]
        public int Value { get; set; }

        [Column("for_event")]
        public int AnnoucementId { get; set; }

        public AnnoucementRecord Annoucement { get; set; }
    }
}
