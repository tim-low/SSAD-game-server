using Game_Server.Model;
using System;
namespace Game_Server.Network
{
    public class CmdTurnPlayerUseItem
    {
        public readonly InventoryItem Item;
        public readonly string Victim;
        public readonly BoardTile Tile;

        public CmdTurnPlayerUseItem(Packet packet)
        {
            Item = new InventoryItem();
            Item.Item = (ItemType)packet.Reader.ReadInt32();
            Victim = packet.Reader.ReadUnicodeStatic(44);
            Tile = new BoardTile();
            Tile.Deserialize(packet.Reader);
        }
    }
}
