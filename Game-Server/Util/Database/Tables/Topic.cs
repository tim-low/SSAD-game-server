using Game_Server.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Game_Server.Controller.Database.Tables
{
    [Table("topic")]
    public class Topic
    {
        [Column("topic_id")]
        public int Id { get; set; }

        [Column("topic_name")]
        public string Name { get; set; }

        public ICollection<Question> Questions { get; set; }

        public ICollection<Mastery> Masteries { get; set; }

        public ICollection<Score> Scores { get; set; }
    }
}
