using Game_Server.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Game_Server.Network
{
    public class CreateSessionAck : OutPacket
    {
        public string SessionCode;
        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.CreateSessionAck);
        }

        public override int ExpectedSize()
        {
            return 6;
        }

        public override byte[] GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new SerializeWriter(ms))
                {
                    sw.WriteTextStatic(SessionCode, 6);
                }
                return ms.ToArray();
            }
        }
    }
}
