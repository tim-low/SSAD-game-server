using Game_Server.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Game_Server.Network
{
    public class SymKeyAnswer : OutPacket
    {
        public byte[] Key;
        public byte[] IV;

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.SymKeyAck);
        }

        public override int ExpectedSize()
        {
            return 50;
        }

        public override byte[] GetBytes()
        {
            using(MemoryStream ms = new MemoryStream())
            {
                using (SerializeWriter sw = new SerializeWriter(ms))
                {
                    sw.Write(Key);
                    sw.Write(IV);
                }
                return ms.ToArray();
            }
        }
    }
}
