using Game_Server.Model.Misc;
using System;
namespace Game_Server.Network
{
    public class CmdChatMessage
    {
        public Message Message;
        public CmdChatMessage(Packet packet)
        {
            Message = new Message()
            {
                Sender = packet.Sender
            };
            Message.Deserialize(packet.Reader);
        }
    }
}
