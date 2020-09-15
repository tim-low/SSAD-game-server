using System;
namespace Game_Server.Network
{
    /// <summary>
    /// CmdSamplePacket contains sample code for processing the byte[] packet into a packet format that we know.
    /// </summary>
    public class CmdSamplePacket
    {
        // For incoming packet (client -> server), we use readonly attribute so that the value can't be changed unless
        // initialization
        public readonly bool IsTrue;
        public readonly string Message;

        public CmdSamplePacket(Packet packet)
        {
            // Inside packet, you can call the SerializeReader variable to read what is in the buffer
            // Order of the read is important, how you write is how you read.
            IsTrue = packet.Reader.ReadBoolean();
            // There are multiple ways to read text, depending on how you write them over the network stream
            Message = packet.Reader.ReadUnicodePrefixed();
        }
    }
}
