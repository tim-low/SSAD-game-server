using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Game_Server.Controller.Database.Tables
{
    [Table("inventory")]
    public class Inventory
    {
        [Key]
        [Column("inventory_id")]
        public int Id { get; set; }
        [Column("head")]
        public byte Head { get; set; }

        [Column("top")]
        public byte Shirt { get; set; }

        [Column("bottom")]
        public byte Pant { get; set; }

        [Column("shoe")]
        public byte Shoe { get; set; }

        [Column("user_id")]
        public string AccountId { get; set; }
        public Account Account { get; set; }
    }
}
