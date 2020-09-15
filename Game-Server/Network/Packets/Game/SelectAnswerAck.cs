using Game_Server.Model;
using Game_Server.Util;
using System;
using System.IO;

namespace Game_Server.Network
{
    public class SelectAnswerAck : OutPacket
    {
        public Reward[] Rewards;

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.SelectAnswerAck);
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
                    sw.Write(Rewards.Length);
                    foreach(Reward r in Rewards)
                    {
                        sw.Write(r);
                    }
                }
                return ms.ToArray();
            }
        }
    }
}
