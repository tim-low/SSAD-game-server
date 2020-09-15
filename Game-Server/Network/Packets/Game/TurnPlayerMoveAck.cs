using Game_Server.Model;
using Game_Server.Util;
using System;
using System.IO;

namespace Game_Server.Network
{
    public class TurnPlayerMoveAck : OutPacket
    {
        public bool IsLegalMove;
        public string Token;
        public BoardTile NewPos;
        public BoardDirection Direction;

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.TurnPlayerMoveAck);
        }

        public override int ExpectedSize()
        {
            return base.ExpectedSize();
        }

        public override byte[] GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new SerializeWriter(ms))
                {
                    sw.Write(IsLegalMove);
                    sw.WriteTextStatic(Token, 44);
                    sw.Write(NewPos);
                    sw.Write((int)Direction);
                }
                return ms.ToArray();
            }
        }
    }
}
