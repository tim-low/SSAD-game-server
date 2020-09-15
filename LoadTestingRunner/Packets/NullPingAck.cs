using Game_Server.Network;
using Game_Server.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LoadTestingRunner
{
    public class NullPingAck : OutPacket
    {
        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.NullPingAck);
        }

        public override int ExpectedSize()
        {
            return 4;
        }

        public override byte[] GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new SerializeWriter(ms))
                {
                    sw.Write(0);
                }
                return ms.ToArray();
            }
        }
    }
}
