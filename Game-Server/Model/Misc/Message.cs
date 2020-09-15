using System;
using System.Diagnostics.CodeAnalysis;
using Game_Server.Network;
using Game_Server.Util;

namespace Game_Server.Model.Misc
{
    /// <summary>
    /// Message is the object containing Text Message in which the User sent. It also store information pertaining to the timestamp
    /// and the type of message which will indicate which section of the server the message will be broadcasted to.
    /// </summary>
    public class Message : IEquatable<Message>, ISerializable
    {
        public GameClient Sender;
        /// <summary>
        /// type of the message in where it will be broadcasted to
        /// </summary>
        public MessageType Type { get; private set; }
        /// <summary>
        /// content of the message
        /// </summary>
        public string Text { get; private set; }
        /// <summary>
        /// when the message was sent, to check and verify it is not a spam message
        /// </summary>
        public DateTime Tick { get; private set; }

        public string TargetTo { get; private set; }

        /// <summary>
        /// Default constructor
        /// Used normally for receiving message from client
        /// </summary>
        public Message()
        {

        }

        /// <summary>
        /// Constructor to set the Type and Text message from the server, this is used normally for outgoing messages from the server
        /// </summary>
        /// <param name="type"></param>
        /// <param name="text"></param>
        public Message(GameClient client, MessageType type, string text)
        {
            this.Sender = client;
            this.Type = type;
            this.Text = text;
            this.Tick = DateTime.Now;
        }

        /// <summary>
        /// Check if an message object is equal to another message object using the Text.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals([AllowNull] Message other)
        {
            if (other == null)
                return false;
            return this.Text.Equals(other.Text);
        }

        /// <summary>
        /// Serialize the content, so we can send this object through the network stream
        /// </summary>
        /// <param name="writer"></param>
        public void Serialize(SerializeWriter writer)
        {
            writer.Write((byte)Type);
            writer.WriteText(Text);
            writer.Write(Tick.Ticks);
        }

        /// <summary>
        /// Deserialize the content from the network stream into an Message object
        /// </summary>
        /// <param name="reader"></param>
        public void Deserialize(SerializeReader reader)
        {
            Type = (MessageType)reader.ReadByte();
            Text = reader.ReadUnicodePrefixed();
            Tick = new DateTime(reader.ReadInt64());
        }

        public override string ToString()
        {
            return String.Format("[{0}]{1}: {2}", Tick.ToString("hh:mm"), Sender.Character.Name, Text); 
        }
    }

    /// <summary>
    /// MessageType determine which section of the server should the message be broacasted to
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// Send to lobby only
        /// </summary>
        LOBBY,
        /// <summary>
        /// Send to room only
        /// </summary>
        ROOM,
        /// <summary>
        /// Send to game only
        /// </summary>
        GAME,
        /// <summary>
        /// Send to everyone as long as they are connected
        /// </summary>
        WORLD
    }
}
