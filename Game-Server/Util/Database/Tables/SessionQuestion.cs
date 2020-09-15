using Game_Server.Controller.Database.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Game_Server.Util.Database.Tables
{
    [Table("session_question")]
    public class SessionQuestion
    {
        [Column("session_id")]
        public string SessionId { get; set; }
        [Column("question_id")]
        public int QuestionId { get; set; }
        public Question Question { get; set; }
    }
}
