using System;
using System.Collections.Generic;
using System.Text;

namespace Game_Server.Network
{
    public class CmdGloryboard
    {
        public readonly int PageNum;
        public readonly int EntriesNum;
        public CmdGloryboard(Packet packet)
        {
            PageNum = packet.Reader.ReadInt32();
            EntriesNum = packet.Reader.ReadInt32();
        }
    }
}
