using System;
using System.Collections.Generic;
using System.Text;

namespace Game_Server.Network
{ 
    public class CmdCreateSession : InPacket
    {
        public readonly int QuizId;
        public CmdCreateSession(Packet packet) : base(packet)
        {
            QuizId = Reader.ReadInt32();
        }
    }
}
