using System;
using System.IO;
using Game_Server.Util;

namespace Game_Server.Network
{
    /// <summary>
    /// Notify other player who's turn it is
    /// </summary>
    public class NotifyPlayerTurnAck : OutPacket
    {
        public string CurrentPlayer;
        public int Duration;

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.NotifyPlayerTurnAck);
        }

        public override int ExpectedSize()
        {
            return 44;
        }

        public override byte[] GetBytes()
        {
            using(var ms = new MemoryStream())
            {
                using (var sw = new SerializeWriter(ms))
                {
                    sw.WriteTextStatic(CurrentPlayer, 44);
                    sw.Write(Duration);

                }
                return ms.ToArray();
            }
        }
    }
}
