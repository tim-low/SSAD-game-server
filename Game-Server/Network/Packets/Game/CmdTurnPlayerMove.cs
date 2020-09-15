using System;
using Game_Server.Model;

namespace Game_Server.Network
{
    public class CmdTurnPlayerMove
    {
        public readonly string Token;
        public readonly BoardDirection Direction;

        public CmdTurnPlayerMove(Packet packet)
        {
            Token = packet.Reader.ReadUnicodeStatic(44);
            Direction = (BoardDirection)packet.Reader.ReadInt32();
        }
    }
}
