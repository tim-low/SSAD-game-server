using System;
using System.IO;
using Game_Server.Model;
using Game_Server.Util;

namespace Game_Server.Network
{
    // Sent room details to players who just joined room
    public class RoomDetailsAck : OutPacket
    {
        public Room Room;

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.RoomDetailsAck);
        }

        public override int ExpectedSize() => 36 * (Room.Clients.Count + 1);

        public override byte[] GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new SerializeWriter(ms))
                {
                    sw.Write(Room.Clients.Count);
                    foreach (GameClient client in Room.Clients)
                    {
                        sw.WriteTextStatic(client.Character.Token, 44);
                        sw.Write(client.Character);
                    }
                    sw.WriteTextStatic(Room.Owner.Character.Token, 44);
                }
                return ms.ToArray();
            }
        }
    }
}
