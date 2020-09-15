using System;
namespace Game_Server.Network.Auth
{
    public class UserAuthPacket
    {
        public readonly string Username;
        public readonly string Password;

        public UserAuthPacket(Packet packet)
        {
            Username = packet.Reader.ReadUnicodeStatic(13).Trim();
            Password = packet.Reader.ReadUnicodeStatic(40).Trim();
        }
    }
}
