using Game_Server.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Game_Server.Network
{
    public class UserRegAck : OutPacket
    {
        public string Token;

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.UserRegAck);
        }

        public override int ExpectedSize()
        {
            return base.ExpectedSize();
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
