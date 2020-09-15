using System;
using System.IO;
using Game_Server.Model;
using Game_Server.Util;

namespace Game_Server.Network
{
    // Sent to player when they leave room
    class LeaveRoomAck : OutPacket
    {
        public Lobby Lobby;

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.LeaveRoomAck);
        }

        public override int ExpectedSize() => 45;

        public override byte[] GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new SerializeWriter(ms))
                {
                    sw.Write(Lobby.RoomManager.GetRooms().Count);
                    foreach (Room r in Lobby.RoomManager.GetRooms())
                    {
                        sw.Write(r);
                    }
                }
                return ms.ToArray();
            }
        }
    }
}
