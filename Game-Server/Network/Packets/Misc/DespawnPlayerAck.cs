using System;
using System.IO;
using Game_Server.Util;

namespace Game_Server.Network
{
    public class DespawnPlayerAck : OutPacket
    {
        public string Token;

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.DespawnPlayerAck);
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
