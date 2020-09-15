using System;
using System.ComponentModel.DataAnnotations.Schema;
using Game_Server.Controller.Database.Tables;

namespace Game_Server.Controller.Database.Tables
{
    [Table("question_quiz")]
    public class QuestionQuiz
    {
        [Column("quiz_id")]
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }

        [Column("question_id")]
        public int QuestionId { get; set; }
        public Question Question { get; set; }
    }
}
