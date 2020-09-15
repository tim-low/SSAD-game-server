using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Game_Server.Network
{
    class CmdRoomPlayerMove
    {
        public readonly string Token;
        public readonly Vector3 StartPos;
        public readonly Vector3 TargetPos;

        public CmdRoomPlayerMove(Packet packet)
        {
            Token = packet.Reader.ReadUnicodeStatic(44);
            StartPos = packet.Reader.ReadVec3();
            TargetPos = packet.Reader.ReadVec3();
        }
    }
}
