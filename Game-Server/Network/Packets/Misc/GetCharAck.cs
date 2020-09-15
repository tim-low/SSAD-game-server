using Game_Server.Model;
using Game_Server.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Game_Server.Network
{
    public class GetCharAck : OutPacket
    {
        public byte Head;
        public byte Shirt;
        public byte Pant;
        public byte Shoe;
        public int ChestCount;

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.GetCharAck);
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
                    sw.Write(Head);
                    sw.Write(Shirt);
                    sw.Write(Pant);
                    sw.Write(Shoe);
                    sw.Write(ChestCount);
                }
                return ms.ToArray();
            }
        }
    }
}
