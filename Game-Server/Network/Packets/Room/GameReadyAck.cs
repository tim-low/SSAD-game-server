using System;
using System.IO;
using Game_Server.Model;
using Game_Server.Util;
using System.Numerics;

namespace Game_Server.Network
{
    // Send to everyone in room to display change of status of a particular player in the room.
    public class ReadyStatusAck : OutPacket
    {
        public string Token;
        public bool IsReady;

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.ReadyStatusAck);
        }

        public override int ExpectedSize() => 45;

        public override byte[] GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new SerializeWriter(ms))
                {
                    sw.WriteTextStatic(Token, 44);
                    sw.Write(IsReady);
                }
                return ms.ToArray();
            }
        }
    }
}
