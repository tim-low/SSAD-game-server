using System;
namespace Game_Server.Network
{
    public class CmdLeaderboard
    {
        public readonly string Token;
        public readonly int PageNum;
        public readonly int PageSize;
        public readonly int LifeCycleStage;

        public CmdLeaderboard(Packet packet)
        {
            Token = packet.Reader.ReadUnicodeStatic(44);
            PageNum = packet.Reader.ReadInt32();
            PageSize = packet.Reader.ReadInt32();
            LifeCycleStage = packet.Reader.ReadInt32();
        }
    }
}
