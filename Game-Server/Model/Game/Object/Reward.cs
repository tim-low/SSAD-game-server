using System;
using System.Collections.Generic;
using System.Text;
using Game_Server.Util;

namespace Game_Server.Model
{
    public class Reward : ISerializable
    {
        public string Token { get; set; }
        public bool IsCorrect { get; set; }
        public InventoryItem Item { get; set; }
        public int Step { get; set; }
        public bool Timeout { get; set; }

        public void Serialize(SerializeWriter writer)
        {
            writer.WriteTextStatic(Token, 44);
            writer.Write(IsCorrect);
            writer.Write(Step);
            writer.Write((int)Item.Item);
            writer.Write(Timeout);
        }
    }
}
