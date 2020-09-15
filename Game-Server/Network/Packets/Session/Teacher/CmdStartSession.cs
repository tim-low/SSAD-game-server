using System;
using System.Collections.Generic;
using System.Text;

namespace Game_Server.Network
{
    public class CmdStartSession : InPacket
    {
        public readonly string SessionCode;
        public CmdStartSession(Packet packet) : base(packet)
        {
            SessionCode = Reader.ReadUnicodeStatic(6);
        }
    }
}
