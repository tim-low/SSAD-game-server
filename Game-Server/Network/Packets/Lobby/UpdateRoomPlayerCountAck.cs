using System;
using System.IO;
using Game_Server.Model;
using Game_Server.Util;

namespace Game_Server.Network
{
    // Sent to others in lobby when number of people in the room change by joining or leaving
    class UpdateRoomPlayerCountAck : OutPacket
    {
        public Room Room;

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.UpdateRoomPlayerCountAck);
        }

        public override int ExpectedSize() => 40;

        public override byte[] GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new SerializeWriter(ms))
                {
                    sw.WriteTextStatic(Room.GetIdentifier(), 36);
                    sw.Write(Room.Clients.Count);
                    Log.Info("Room Id: {0}, Count: {1}", Room.GetIdentifier(), Room.Clients.Count);
                }
                return ms.ToArray();
            }
        }
    }
}
