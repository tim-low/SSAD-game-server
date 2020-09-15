using System;
using System.IO;
using Game_Server.Model;
using Game_Server.Util;

namespace Game_Server.Network
{
    // Sent to others in room when a player leaves
    class PlayerHasLeftRoomAck : OutPacket
    {
        public Character Character { get; set; }
        public string Token { get; set; }
        public bool HasOwnerChange { get; set; }
        public string Owner { get; set; }

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.PlayerHasLeftRoomAck);
        }

        public override int ExpectedSize() => 36;

        public override byte[] GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new SerializeWriter(ms))
                {
                    sw.WriteTextStatic(Token, 44);
                    sw.Write(Character);
                    sw.Write(HasOwnerChange);
                    if(HasOwnerChange)
                    {
                        sw.WriteTextStatic(Owner, 44);
                    }
                }
                return ms.ToArray();
            }
        }
    }
}
