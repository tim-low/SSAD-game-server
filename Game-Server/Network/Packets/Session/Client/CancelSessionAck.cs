using Game_Server.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Game_Server.Network
{
    public class CancelSessionAck : OutPacket
    {
        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.CancelSessionAck);
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
                    sw.Write(true);
                }
                return ms.ToArray();
            }
        }
    }
}
