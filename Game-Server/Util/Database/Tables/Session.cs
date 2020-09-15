using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Game_Server.Controller.Database.Tables
{
    [Table("session")]
    public class Session
    {
        [Column("session_id")]
        public string Id { get; set; }

        [Column("user_id")]
        public string AccountId { get; set; }
        public Account Account { get; set; }

        [Column("quiz_id")]
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }
    }
}
