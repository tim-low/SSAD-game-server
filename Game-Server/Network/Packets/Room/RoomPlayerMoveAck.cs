using System;
using System.IO;
using Game_Server.Model;
using Game_Server.Util;
using System.Numerics;

namespace Game_Server.Network
{
    // Sent to player who chose to move
    public class RoomPlayerMoveAck : OutPacket
    {
        public string Token;
        public Vector3 StartPos;
        public Vector3 TargetPos;
        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.RoomPlayerMoveAck);
        }

        public override int ExpectedSize() => 44 + 12 * 2;

        public override byte[] GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new SerializeWriter(ms))
                {
                    sw.WriteTextStatic(Token, 44);
                    sw.Write(StartPos);
                    sw.Write(TargetPos);
                }
                return ms.ToArray();
            }
        }
    }
}
