using System;
using System.Collections.Generic;
using System.Text;

namespace Game_Server.Network
{
    class CmdWorldSelect
    {
        public readonly string Token;
        public readonly int LobbyId;

        public CmdWorldSelect(Packet packet)
        {
            Token = packet.Reader.ReadUnicodeStatic(44);
            LobbyId = packet.Reader.ReadInt32();
        }
    }
}
