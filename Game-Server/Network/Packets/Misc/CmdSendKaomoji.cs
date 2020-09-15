using System;
using System.Collections.Generic;
using System.Text;

namespace Game_Server.Network
{
    public class CmdSendKaomoji
    {
        public readonly string Token;
        public readonly int KaomojiId;

        public CmdSendKaomoji(Packet packet)
        {
            Token = packet.Reader.ReadUnicodeStatic(44);
            KaomojiId = packet.Reader.ReadInt32();
        }
    }
}
