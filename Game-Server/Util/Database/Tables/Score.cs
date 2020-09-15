using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Game_Server.Controller.Database.Tables
{
    [Table("score")]
    public class Score
    {
        [Key]
        [Column("score_id")]
        public int Id { get; set; }
        [Column("points")]
        public int Point { get; set; }
        [Column("created_on")]
        public DateTime CreatedOn { get; set; }
        
        [Column("achieved_by")]
        public string AccountId { get; set; }
        public Account CreatedBy { get; set; }

        [Column("topic_id")]
        public int TopicId { get; set; }
        public Topic Topic { get; set; }
    }
}
