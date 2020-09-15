using System;
using System.IO;
using Game_Server.Util;

namespace Game_Server.Network
{
    /// <summary>
    /// SampleAck is the outgoing packet (server -> client)
    /// You'll need to override at least two methods.
    ///     - CreatePacket()
    ///     - GetBytes()
    /// The third method that you can override is ExpectedSize() but that is for writing test cases.
    /// </summary>
    public class SampleAck : OutPacket
    {
        public int StatusCode;
        public string Message;
        public SampleObject Object;

        public override Packet CreatePacket()
        {
            // Create a packet with unique identifier of SampleAck and the packet buffer contain StatusCode and Message
            return base.CreatePacket(Packets.SampleAck);
        }

        public override int ExpectedSize()
        {
            // You know that int is 32 bit which mean it is 4 bytes and Message is depend on the length of the message
            return 4 + Message.Length;
        }

        public override byte[] GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new SerializeWriter(ms))
                {
                    // Write the data you want to the underlying buffer
                    sw.Write(StatusCode);
                    sw.WriteText(Message);
                    // with a serializable object, you write it like this instead
                    Object.Serialize(sw);
                    // Client that receive this should read the Status Code first then the Message and then Object.
                }
                return ms.ToArray();
            }
        }


    }
}
