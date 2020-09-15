using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Game_Server.Util.Database.Tables
{
    [Table("annoucements")]
    public class AnnoucementRecord
    {
        [Key]
        [Column("annoucement_id")]
        public int Id { get; set; }
        [Column("annoucement_name")]
        public string Name { get; set; }
        [Column("annoucement_desc")]
        public string Message { get; set; }
        [Column("annoucement_image")]
        public string Image { get; set; }

        [Column("is_active")]
        public int IsActive { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }
        [Column("end_date")]
        public DateTime EndDate { get; set; }
        public ICollection<EventRecord> Events { get; set; }
    }
}
