using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Game_Server.Controller.Database.Tables
{
    [Table("question_attempted")]
    public class QuestionAttempted
    {
        [Key]
        [Column("qa_id")]
        public int Id { get; set; }

        [Column("answer_id")]
        public int AnswerId { get; set; }
        public Answer Answer { get; set; }

        [Column("attempted_on")]
        public DateTime AttemptedOn { get; set; }

        [Column("question_id")]
        public int QuestionId { get; set; }
        public Question Question { get; set; }

        [Column("attempted_by")]
        public string AttemptedBy { get; set; }
        public Account Account { get; set; }

        [Column("attempted_for")]
        public string AttemptedFor { get; set; }


    }
}
