using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Game_Server.Controller.Database.Tables
{
    /// <summary>
    /// Account schema in EFCore
    /// </summary>
    [Table("user")]
    public class Account
    {
        [Key]
        [Column("user_id")]
        public string Id { get; set; }

        [Column("username")]
        public string Username { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("fname")]
        public string FullName { get; set; }
        [Column("class")]
        public string Class { get; set; }
        [Column("year")]
        public int Year { get; set; }
        [Column("semester")]
        public int Semester { get; set; }
        [Column("password")]
        public string Password { get; set; }
        [Column("salt")]
        public string Salt { get; set; }
        [Column("permission")]
        public int Permission { get; set; }

        [Column("is_logged_in")]
        public int IsLoggedIn { get; set; }

        [Column("creation_date")]
        public DateTime CreationDate { get; set; }

        [Column("last_logged_in")]
        public DateTime LastLoggedIn { get; set; }
        
        public Character Character { get; set; }

        public Inventory Inventory { get; set; }

        public ICollection<Score> Scores { get; set; }

        public ICollection<Mastery> Masteries { get; set; }

        public ICollection<Session> Sessions { get; set; }

        public ICollection<QuestionAttempted> QuestionAttempteds { get; set; }
    }
}
