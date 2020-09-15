using System.IO;
using Game_Server.Model;
using Game_Server.Util;

namespace Game_Server.Network
{
    // Sent to other players when another player creates a room
    public class RoomCreatedAck : OutPacket
    {
        public Room Room;

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.RoomCreatedAck);
        }

        public override int ExpectedSize() => 44;

        public override byte[] GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new SerializeWriter(ms))
                {
                    sw.Write(Room);
                }
                return ms.ToArray();
            }
        }
    }
}
