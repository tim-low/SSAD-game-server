using System;
using System.IO;
using Game_Server.Util;

namespace Game_Server.Network
{
    /// <summary>
    /// Base Class for any outgoing packets
    /// </summary>
    public class OutPacket
    {

        public virtual Packet CreatePacket()
        {
            return null;
        }

        public virtual int ExpectedSize()
        {
            return 0;
        }

        protected Packet CreatePacket(ushort packetId)
        {
            var ack = new Packet(packetId);
            // ack.Writer.Write((ushort)(GetBytes()).Length);
            ack.Writer.Write(GetBytes());
            return ack;
        }

        public virtual byte[] GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var bs = new SerializeWriter(ms))
                {
                }
                return ms.ToArray();
            }
        }

    }
}
