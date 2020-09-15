using Game_Server.Model.Misc;
using Game_Server.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Game_Server.Network
{
    public class ChatMessageAnswer : OutPacket
    {
        public Message Message;

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.ChatMsgAck);
        }

        public override int ExpectedSize()
        {
            return 2 + (Message.ToString().Length); // 20 is Character Length, 10 is type of message // Include padding
        }

        public override byte[] GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new SerializeWriter(ms))
                {
                    sw.Write((byte)Message.Type);
                    sw.WriteText(Message.ToString());
                }
                return ms.ToArray();
            }
        }
    }
}
