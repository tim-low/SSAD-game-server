using System;
using System.IO;
using Game_Server.Model;
using Game_Server.Util;

namespace Game_Server.Network
{
    public class TurnPlayerUseItemAck : OutPacket
    {
        public string Caster;
        public string Victim;
        public InventoryItem Item;
        public BoardTile Tile;

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.TurnPlayerUseItemAck);
        }

        public override int ExpectedSize()
        {
            return base.ExpectedSize();
        }

        public override byte[] GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new SerializeWriter(ms))
                {
                    Log.Debug("Caster: {0}", Caster);
                    sw.WriteTextStatic(Caster, 44);
                    Log.Debug("Victim: {0}", Victim);
                    sw.WriteTextStatic(Victim, 44);
                    Log.Debug("Item: {0}", (int)Item.Item);
                    sw.Write(Item);
                    Log.Debug("Tile: {0}, {1}, {2}", Tile.X, Tile.Y, Tile.Color);
                    sw.Write(Tile);
                }
                return ms.ToArray();
            }
        }
    }
}
