using System;
using System.Collections.Generic;
using System.Text;

namespace Game_Server.Network
{
    public class CmdTurnEnd
    {
        public readonly string Token;
        
        public CmdTurnEnd(Packet packet)
        {
            Token = packet.Reader.ReadUnicodeStatic(44);
        }
    }
}
