using Game_Server.Model.Misc;
using Game_Server.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Game_Server.Network
{
    public class NoticeDataAck : OutPacket
    {
        public Annoucement[] Annoucements;
        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.NoticeDataAck);
        }

        public override byte[] GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new SerializeWriter(ms))
                {
                    sw.Write(Annoucements.Length);
                    foreach(var annoucement in Annoucements)
                    {
                        sw.Write(annoucement);
                    }
                }
                return ms.ToArray();
            }
        }
    }
}
