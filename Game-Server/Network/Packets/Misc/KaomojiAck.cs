using Game_Server.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Game_Server.Network
{
    public class KaomojiAck : OutPacket
    {
        public string Token;
        public int KaomojiId;

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.SendKaomojiAck);
        }

        public override int ExpectedSize()
        {
            return 48;
        }

        public override byte[] GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new SerializeWriter(ms))
                {
                    sw.WriteTextStatic(Token, 44);
                    sw.Write(KaomojiId);
                }
                return ms.ToArray();
            }
        }

    }
}
