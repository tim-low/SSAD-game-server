using System;
using System.Collections.Generic;
using System.IO;
using Game_Server.Model;
using Game_Server.Util;

namespace Game_Server.Network
{
    public class EndGameAck : OutPacket
    {
        public List<GameScoreModel> Scores;

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.EndGameAck);
        }

        public override int ExpectedSize()
        {
            return 4 + (Scores.Count * 52);
        }

        public override byte[] GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new SerializeWriter(ms))
                {
                    sw.Write(Scores.Count);
                    foreach(GameScoreModel model in Scores)
                    {
                        sw.Write(model);
                    }
                }
                return ms.ToArray();
            }
        }
    }
}
