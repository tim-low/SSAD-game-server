using System;
using System.IO;
using Game_Server.Model;
using Game_Server.Util;

namespace Game_Server.Network
{
    // Sent to others in lobby when number of people in the room change by joining or leaving
    class UpdateRoomStatusAck : OutPacket
    {
        public Room Room;

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.UpdateRoomStatusAck);
        }

        public override int ExpectedSize() => 37;

        public override byte[] GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new SerializeWriter(ms))
                {
                    sw.WriteTextStatic(Room.GetIdentifier(), 36);
                    sw.Write(Room.IsInGame);
                }
                return ms.ToArray();
            }
        }
    }
}
