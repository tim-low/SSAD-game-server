using System;
using System.Collections.Generic;
using System.Text;

namespace Game_Server.Network
{
    class CmdLeaveLobby
    {
        public readonly string Token;

        public CmdLeaveLobby(Packet packet)
        {
            Token = packet.Reader.ReadUnicodeStatic(44);
        }
    }
}
