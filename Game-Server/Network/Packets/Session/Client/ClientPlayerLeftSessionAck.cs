using Game_Server.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Game_Server.Network
{
    public class ClientPlayerLeftSessionAck : OutPacket
    {
        public string Token { get; set; }
        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.ClientPlayerLeftSessionAck);
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
