using Game_Server.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Game_Server.Controller.Database.Tables
{
    [Table("question")]
    public class Question
    {
        [Column("question_id")]
        [JsonProperty("id")]
        public int Id { get; set; }

        [Column("question_text")]
        [JsonProperty("question")]
        public string Text { get; set; }

        [Column("difficulty")]
        [JsonIgnore]
        public int Difficulty { get; set; }

        [Column("is_custom")]
        [JsonIgnore]
        public int IsCustom { get; set; }

        [Column("topic_id")]
        [JsonProperty("topicId")]
        public int TopicId { get; set; }

        [Column("created_by")]
        [JsonIgnore]
        public string CreatedBy { get; set; }

        [JsonIgnore]
        public Topic CategorizedBy { get; set; }

        [JsonIgnore]
        public ICollection<QuestionAttempted> Questions { get; set; }

        [JsonProperty("answers")]
        public ICollection<Answer> Answers { get; set; }

        [JsonIgnore]
        public ICollection<QuestionQuiz> Quizzes { get; set; }
    }
}
