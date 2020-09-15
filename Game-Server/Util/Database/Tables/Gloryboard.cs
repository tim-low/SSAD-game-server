using Game_Server.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Game_Server.Controller.Database.Tables
{
    [Table("gloryboard")]
    public class Gloryboard : ISerializable
    {
        [Key]
        [Column("glory_id")]
        public int Id { get; set; }

        [Column("date_of_completion")]
        public DateTime CompletionDate { get; set; }

        [Column("account_id")]
        public string AccountId { get; set; }
        public Account Account { get; set; }

        public void Serialize(SerializeWriter writer)
        {
            writer.WriteTextStatic(Account.Username, 13);
            writer.Write(CompletionDate.Ticks);
        }
    }
}
