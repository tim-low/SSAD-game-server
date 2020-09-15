using Game_Server.Model;
using Game_Server.Util;
using System;
using System.IO;

namespace Game_Server.Network
{
    public class StartQuizAck : OutPacket
    {
        public Question Question;
        public int DurationInSec;

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.StartQuizAck);
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
                    sw.Write(Question);
                    sw.Write(DurationInSec);
                }
                return ms.ToArray();
            }
        }
    }
}
