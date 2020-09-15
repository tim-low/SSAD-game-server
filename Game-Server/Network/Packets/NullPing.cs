using System;
using System.IO;
using Game_Server.Util;

namespace Game_Server.Network
{
    public class NullPing : OutPacket
    {
        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.CmdNullPing);
        }

        public override int ExpectedSize()
        {
            return 0;
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
