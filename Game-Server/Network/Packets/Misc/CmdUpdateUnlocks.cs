using System;
using System.Collections.Generic;
using System.Text;

namespace Game_Server.Network
{
    public class CmdUpdateUnlocks
    {
        public readonly string Token;
        /// <summary>
        /// The byte here represent the head customizable the user owned in flags bit
        /// </summary>
        public readonly byte Head;
        /// <summary>
        /// The byte here represent the shirt customizable the user owned in flags bit
        /// </summary>
        public readonly byte Shirt;
        /// <summary>
        /// The byte here represent the pant customizable the user owned in flags bit
        /// </summary>
        public readonly byte Pants;
        /// <summary>
        /// The byte here represent the shoe customizable the user owned in flags bit
        /// </summary>
        public readonly byte Shoes;

        public CmdUpdateUnlocks(Packet packet)
        {
            Token = packet.Reader.ReadUnicodeStatic(44);
            Head = packet.Reader.ReadByte();
            Shirt = packet.Reader.ReadByte();
            Pants = packet.Reader.ReadByte();
            Shoes = packet.Reader.ReadByte();
        }
    }
}
