using System;
namespace Game_Server.Network
{
    public class CmdSymKey
    {

        public readonly byte[] ProtocolVersion;

        public CmdSymKey(Packet packet)
        {
            ProtocolVersion = packet.Reader.ReadBytes(4);

        }
    }
}
