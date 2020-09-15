using System;
using System.IO;
using Game_Server.Util;

namespace Game_Server.Network
{
    /// <summary>
    /// Notify the player that his turn has ended
    /// </summary>
    public class EndPlayerTurnAck : OutPacket
    {
        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.EndPlayerTurnAck);
        }

        public override int ExpectedSize()
        {
            return 4;
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
