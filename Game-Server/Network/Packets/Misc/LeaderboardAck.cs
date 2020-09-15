using System;
using System.IO;
using Game_Server.Model;
using Game_Server.Util;

namespace Game_Server.Network
{
    public class LeaderboardAck : OutPacket
    {

        public int OwnRanking { get; set; }
        public bool IsLastPage { get; set; }
        public Ranking[] Entries { get; set; }

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.LeaderboardAck);
        }

        public override int ExpectedSize()
        {
            return 4 + (Entries.Length * (13 + 4));
        }

        public override byte[] GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new SerializeWriter(ms))
                {
                    sw.Write(OwnRanking);
                    sw.Write(IsLastPage);
                    sw.Write(Entries.Length);
                    foreach(Ranking r in Entries)
                    {
                        sw.Write(r);
                    }
                }
                return ms.ToArray();
            }
        }
    }
}
