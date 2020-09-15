using System;
using System.Collections.Generic;
using System.Text;

namespace Game_Server.Network
{
    class CmdCreateRoom
    {
        public readonly string Token;
        public readonly string RoomName;
        public readonly bool IsLocked;
        public readonly string Password;
        public readonly int NumTurns;      // 5-20

        public CmdCreateRoom(Packet packet)
        {
            Token = packet.Reader.ReadUnicodeStatic(44);
            RoomName = packet.Reader.ReadUnicodeStatic(40);
            IsLocked = packet.Reader.ReadBoolean();
            if (IsLocked)
            {
                Password = packet.Reader.ReadUnicodeStatic(40);
            }
            NumTurns = packet.Reader.ReadByte();
        }
    }
}
