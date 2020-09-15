using System;
using Game_Server.Util;

namespace Game_Server.Network
{
    /// <summary>
    /// All object/model that can be send via the Socket stream need to be a subclass of the ISerializable interface
    /// and you need to determine which variable should be send over to the client
    /// </summary>
    public class SampleObject : ISerializable
    {
        public int x;
        public float y;
        public string z;

        public SampleObject()
        {
        }

        public void Serialize(SerializeWriter writer)
        {
            // Determine how to serialize this object, so that it can be send via the Socket Stream
            writer.Write(x);
            writer.Write(y);
            writer.WriteText(z);
        }
    }
}
