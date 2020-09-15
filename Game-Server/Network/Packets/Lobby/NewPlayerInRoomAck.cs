using System;
using System.IO;
using Game_Server.Model;
using Game_Server.Util;

namespace Game_Server.Network
{
    // Sent to others in the room when a player just entered
    class NewPlayerInRoomAck : OutPacket
    {
        public string Token;
        public Character Character;

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.NewPlayerInRoomAck);
        }

        public override int ExpectedSize() => 36;

        public override byte[] GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new SerializeWriter(ms))
                {
                    // Not sure.
                    sw.WriteTextStatic(Token, 44);
                    sw.Write(Character);
                }
                return ms.ToArray();
            }
        }
    }
}
