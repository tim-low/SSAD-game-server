using System;
using System.IO;
using Game_Server.Model;
using Game_Server.Util;

namespace Game_Server.Network
{
    public class PlayerStatsAck : OutPacket
    {

        public LeaderboardStats LeaderboardStats { get; set; }
        public Experience Experience { get; set; }
        public int GameCount { get; set; }

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.PlayerStatsAck);
        }

        public override int ExpectedSize()
        {
            return (19 * sizeof(int));
        }

        public override byte[] GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new SerializeWriter(ms))
                {
                    sw.Write(Experience);
                    sw.Write(LeaderboardStats);
                    sw.Write(GameCount);
                }
                return ms.ToArray();
            }
        }
    }
}
