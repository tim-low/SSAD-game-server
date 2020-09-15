using System;
using System.Collections.Generic;
using System.Text;

namespace Game_Server.Network
{
    class CmdRoomDetails
    {
        public readonly string Token;
        public readonly string RoomId;

        public CmdRoomDetails(Packet packet)
        {
            Token = packet.Reader.ReadUnicodeStatic(44);
            RoomId = packet.Reader.ReadUnicodeStatic(36);
        }
    }
}
