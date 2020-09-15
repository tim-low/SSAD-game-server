using Game_Server.Util;
using System.IO;

namespace Game_Server.Network
{
    public class UpdateUnlocksAck : OutPacket
    {
        public bool Success;

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.UpdateUnlocksAck);
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
                    sw.Write(Success);
                }
                return ms.ToArray();
            }
        }
    }
}