using System;
using System.Collections.Generic;
using System.IO;
using Game_Server.Model;
using Game_Server.Util;

namespace Game_Server.Network
{
    // Sent to player when they select a world and enter lobby
    class WorldSelectAck : OutPacket
    {
        public Lobby Lobby;

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.WorldSelectAck);
        }

        public override int ExpectedSize() => 4 + Lobby.RoomManager.GetRooms().Count * 44;

        public override byte[] GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new SerializeWriter(ms))
                {
                    sw.Write(Lobby.RoomManager.GetRooms().Count);
                    Log.Info("Lobby ID: {0}, Room Count: {1}", Lobby.TopicId, Lobby.RoomManager.GetRooms().Count);
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