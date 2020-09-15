using System;
using System.Collections.Generic;
using System.Text;

namespace Game_Server.Network
{
    public class CmdJoinSession : InPacket
    {
        public readonly string SessionCode;

        public CmdJoinSession(Packet packet) : base(packet)
        {
            SessionCode = Reader.ReadUnicodeStatic(6);
        }
    }
}
