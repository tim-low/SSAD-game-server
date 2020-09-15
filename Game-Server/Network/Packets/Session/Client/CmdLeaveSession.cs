using System;
using System.Collections.Generic;
using System.Text;

namespace Game_Server.Network
{
    public class CmdLeaveSession : InPacket
    {
        public int Filler { get; set; }

        public CmdLeaveSession(Packet packet) : base(packet)
        {
            // Do nothing if no need to read
        }
    }
}
