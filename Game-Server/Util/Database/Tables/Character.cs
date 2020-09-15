using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Game_Server.Controller.Database.Tables
{
    [Table("character")]
    public class Character
    {
        [Key]
        [Column("character_id")]
        public string Id { get; set; }
        [Column("head")]
        public byte HeadEqp { get; set; }
        [Column("top")]
        public byte TopEqp { get; set; }
        [Column("bottom")]
        public byte BottomEqp { get; set; }
        [Column("shoe")]
        public byte ShoeEqp { get; set; }
        [Column("num_of_chest")]
        public int ChestCount { get; set; }

        [Column("user_id")]
        public string AccountId { get; set; }
        public Account Account { get; set; }
    }
}
