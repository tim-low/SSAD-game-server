using System;
namespace Game_Server.Network
{
    public class CmdInitializeGame
    {
        public readonly string Token;

        public CmdInitializeGame(Packet packet)
        {
            Token = packet.Reader.ReadUnicodeStatic(44);
        }
    }
}
