using System;
namespace Game_Server.Util
{
    /// <summary>
    /// Serializable class to aid in serializing data over the TcpClient
    /// </summary>
    public interface ISerializable
    {
        void Serialize(SerializeWriter writer);
    }
}
