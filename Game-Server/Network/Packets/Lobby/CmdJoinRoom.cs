using System;
using System.Collections.Generic;
using System.Text;

namespace Game_Server.Network
{
    class CmdJoinRoom
    {
        public readonly string Token;
        public readonly string RoomId;
        public readonly bool IsLocked;
        public readonly string Password;

        public CmdJoinRoom(Packet packet)
        {
            Token = packet.Reader.ReadUnicodeStatic(44);
            RoomId = packet.Reader.ReadUnicodeStatic(36);
            IsLocked = packet.Reader.ReadBoolean();
            if (IsLocked)
            {
                Password = packet.Reader.ReadUnicodeStatic(40);
            }
        }
    }
}
