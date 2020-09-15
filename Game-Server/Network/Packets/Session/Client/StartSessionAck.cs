using Game_Server.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Game_Server.Network
{
    public class StartSessionAck : OutPacket
    {
        public bool Success;

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.StartSessionAck);
        }

        public override int ExpectedSize()
        {
            return 1;
        }

        public override byte[] GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new SerializeWriter(ms))
                {
                    sw.Write(Success);
                }
                return ms.ToArray();
            }
        }
    }
}
