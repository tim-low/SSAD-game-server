using System;
namespace Game_Server.Network
{
    public class CmdOpenChest
    {
        public readonly string Token;

        public CmdOpenChest(Packet packet)
        {
            Token = packet.Reader.ReadUnicodeStatic(44);
        }
    }
}
