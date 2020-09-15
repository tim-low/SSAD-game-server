using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Game_Server.Model;
using Game_Server.Util;

namespace Game_Server.Network
{
    public class GetChestRewardAck : OutPacket
    {
        public int AttributeNumber;
        public int ItemNumber;

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.GetChestRewardAck);
        }

        public override int ExpectedSize() => 4;

        public override byte[] GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new SerializeWriter(ms))
                {
                    sw.Write(AttributeNumber);
                    sw.Write(ItemNumber);
                }
                return ms.ToArray();
            }
        }
    }
}
