using Game_Server.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Game_Server.Network
{
    public class GetNoticesAck : OutPacket
    {
        public int[] Notices { get; set; }

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.GetNoticesAck);
        }

        public override int ExpectedSize()
        {
            return 0;
        }

        public override byte[] GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new SerializeWriter(ms))
                {
                    sw.Write(Notices.Length);
                    foreach(var noticeIndex in Notices)
                    {
                        sw.Write(noticeIndex);
                    }
                }
                return ms.ToArray();
            }
        }
    }
}
