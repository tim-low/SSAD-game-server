using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace Game_Server.Controller.Database.Tables
{
    /// <summary>
    /// Answer Entity Model & Json Serialization Model
    /// </summary>
    [Table("answer")]
    public class Answer
    {
        [Column("answer_id")]
        public int Id { get; set; }
        [Column("answer_text")]
        public string Text { get; set; }
        [Column("is_correct")]
        [JsonIgnore]
        [JsonProperty("is_correct")]
        public int IsCorrect { get; set; }


        [Column("question_id")]
        public int QuestionId { get; set; }

        [JsonIgnore]
        public Question AnswerFor { get; set; }
        [JsonIgnore]
        public ICollection<QuestionAttempted> Attempteds { get; set; }
    }
}
