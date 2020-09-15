using Game_Server.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game_Server.Network
{
    /// <summary>
    /// This class basically in charge of creating/updating the character object
    /// </summary>
    public class CmdUpdateChar
    {
        public readonly string Token;
        public readonly byte Head;
        public readonly byte Shirt;
        public readonly byte Pant;
        public readonly byte Shoe;
        public CmdUpdateChar(Packet packet)
        {
            Token = packet.Reader.ReadUnicodeStatic(44).Trim();
            Head = packet.Reader.ReadByte();
            Shirt = packet.Reader.ReadByte();
            Pant = packet.Reader.ReadByte();
            Shoe = packet.Reader.ReadByte();
        }
    }
}
