using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Game_Server.Controller.Database.Tables
{
    [Table("mastery")]
    public class Mastery
    {
        [Column("experience")]
        public int Exp { get; set; }

        [Column("mastery_for")]
        public int TopicId { get; set; }
        public Topic Topic { get; set; }

        [Column("mastery_by")]
        public string AccountId { get; set; }
        public Account Account { get; set; }
        // public Account Account;
    }
}
