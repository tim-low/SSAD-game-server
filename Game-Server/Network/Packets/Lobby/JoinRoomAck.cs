using System;
using System.IO;
using Game_Server.Model;
using Game_Server.Util;

namespace Game_Server.Network
{
    // Sent to player when they join room
    class JoinRoomAck : OutPacket
    {
        public Room Room;

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.JoinRoomAck);
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
