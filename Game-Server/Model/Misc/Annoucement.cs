using Game_Server.Util;
using Game_Server.Util.Database;
using Game_Server.Util.Database.Tables;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game_Server.Model.Misc
{
    public class Annoucement : IDatabase<AnnoucementRecord>, ISerializable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public string ImageUrl { get; set; }
        public byte[] Image { get; set; }
        public bool IsLifeTime { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ICollection<Event> Benefits { get; set; }


        public bool IsActive()
        {
            return IsLifeTime || (DateTime.Now > StartDate && DateTime.Now < EndDate);
        }

        public void FromEntity(AnnoucementRecord item)
        {
            this.Id = item.Id;
            this.Name = item.Name;
            this.Message = item.Message;
            this.ImageUrl = item.Image;
            this.Image = Utilities.DownloadImage(item.Image);
            this.StartDate = item.StartDate;
            this.EndDate = item.EndDate;
            this.IsLifeTime = item.IsActive == 1;
            Benefits = new List<Event>();
            foreach(var ev in item.Events)
            {
                var benefit = new Event();
                benefit.FromEntity(ev);
                Benefits.Add(benefit);
            }
        }

        public AnnoucementRecord ToEntity()
        {
            AnnoucementRecord record = new AnnoucementRecord()
            {
                Id = this.Id,
                Name = this.Name,
                Message = this.Message,
                Image = this.ImageUrl,
                IsActive = this.IsLifeTime == true ? 1 : 0,
                StartDate = this.StartDate,
                EndDate = this.EndDate,

            };
            return record;
        }

        public void Serialize(SerializeWriter writer)
        {
            writer.Write(Id);
            writer.WriteText(Name, false);
            writer.WriteText(Message, false);
            writer.Write(Image.Length);
            writer.Write(Image);
        }
    }
}
