using System;
using System.Collections.Generic;
using System.Text;

namespace Game_Server.Network
{
    public class CmdCancelSession : InPacket
    {
        public string SessionCode { get; set; }
        public CmdCancelSession(Packet packet) : base(packet)
        {
            SessionCode = Reader.ReadUnicodeStatic(6);
        }
    }
}
