using System;
using System.IO;
using Game_Server.Util;

namespace Game_Server.Network
{
    public class LeaveSessionAck : OutPacket
    {
        public string Token;

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.LeaveSessionAck);
        }

        public override int ExpectedSize()
        {
            return 44;
        }

        public override byte[] GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new SerializeWriter(ms))
                {
                    sw.WriteTextStatic(Token, 44);
                }
                return ms.ToArray();
            }
        }
    }
}
