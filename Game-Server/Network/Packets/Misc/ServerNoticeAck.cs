using Game_Server.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Game_Server.Network
{
    public class ServerNoticeAck : OutPacket
    {
        public string EventMessage { get; set; }

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.ServerNoticeAck);
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
                    sw.WriteText(EventMessage, false);
                }
                return ms.ToArray();
            }
        }
    }
}
