using System;
using System.IO;
using Game_Server.Util;

namespace Game_Server.Network
{
    public class StartPlayerTurnAck : OutPacket
    {
        public string Token;
        public int Duration;

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.StartPlayerTurnAck);
        }

        public override int ExpectedSize()
        {
            return 44 + 4;
        }

        public override byte[] GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new SerializeWriter(ms))
                {
                    sw.WriteTextStatic(Token, 44);
                    sw.Write(Duration);
                }
                return ms.ToArray();
            }
        }
    }
}
