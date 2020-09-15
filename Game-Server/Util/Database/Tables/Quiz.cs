using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Game_Server.Controller.Database.Tables
{
    [Table("quiz")]
    public class Quiz
    {
        [Key]
        [Column("quiz_id")]
        public int Id { get; set; }
        [Column("quiz_name")]
        [JsonProperty("quizName")]
        public string Name { get; set; }
        [Column("is_custom")]
        public int IsCustom { get; set; }

        [Column("created_by")]
        public string CreatedBy { get; set; }

        [JsonIgnore]
        public ICollection<QuestionQuiz> Questions { get; set; }

        [JsonProperty("numQuestions")]
        public int NumOfQuestions
        {
            get { return Questions.Count; }
        }

        public ICollection<Session> Sessions { get; set; }
        
    }
}
