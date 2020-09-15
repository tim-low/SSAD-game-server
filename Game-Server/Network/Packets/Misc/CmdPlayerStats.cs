using System;
namespace Game_Server.Network
{
    public class CmdPlayerStats
    {

        public readonly string Token;

        public CmdPlayerStats(Packet packet)
        {
            Token = packet.Reader.ReadUnicodeStatic(44);
        }
    }
}
