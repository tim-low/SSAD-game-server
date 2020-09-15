using System;
using Game_Server.Util;

namespace Game_Server.Model
{
    /// <summary>
    /// This is the tile for the 8x8 Board
    /// </summary>
    public class BoardTile : ISerializable
    {
        public byte X { get; private set; }
        public byte Y { get; private set; }
        public byte Color { get; private set; }
        public bool Inaccessible { get; private set; }
        public byte Lock { get; private set; }

        /// <summary>
        /// Constructor to initialize the X and Y value of BoardTile since they have private setter,
        /// thus effectively making them a readonly with less restriction
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        public BoardTile(int X = 0, int Y = 0)
        {
            this.X = (byte) X;
            this.Y = (byte) Y;
            this.Color = 0;
            this.Inaccessible = false;
            this.Lock = 0;
        }

        /// <summary>
        /// Write all the data pertaining to the BoardTile that we want to send over the network stream
        /// </summary>
        /// <param name="writer">the SerializeWriter object which we going to write the value to</param>
        public void Serialize(SerializeWriter writer)
        {
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Color);
        }

        public bool IsLocked()
        {
            return Lock > 0;
        }

        public bool IsInaccessible()
        {
            return Inaccessible;
        }

        public BoardTile LockTile()
        {
            Lock = 2;
            return this;
        }

        public BoardTile Unlock()
        {
            Lock -= 1;
            return this;
        }

        public BoardTile SetColor(int color)
        {
            Color = (byte) color;
            return this;
        }

        public BoardTile SetInaccessible(bool value)
        {
            this.Inaccessible = value;
            return this;
        }
        /// <summary>
        /// Read the value in the order of which we serialize it before sending over the networks stream
        /// </summary>
        /// <param name="reader"the SerializeReader object which we going to read the value></param>
        public void Deserialize(SerializeReader reader)
        {
            X = reader.ReadByte();
            Y = reader.ReadByte();
            Color = reader.ReadByte();
        }
    }
}
