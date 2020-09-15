using System;
using System.Diagnostics.CodeAnalysis;
using Game_Server.Util;

namespace Game_Server.Model
{
    /// <summary>
    /// InventoryItem object for the Player
    /// implement IEquatable to make it compare on the list
    /// </summary>
    public class InventoryItem : ISerializable, GameObject, IEquatable<InventoryItem>
    {
        public ItemType Item { get; set; }

        public InventoryItem()
        {
            
        }

        public void Serialize(SerializeWriter writer)
        {
            writer.Write((int)Item);
        }

        public static InventoryItem Spawn(int effect)
        {
            InventoryItem item = new InventoryItem();
            item.Item = (ItemType)effect;
            return item;
        }

        public string GetIdentifier()
        {
            return this.Item.ToString();
        }

        public bool Equals([AllowNull] InventoryItem other)
        {
            if (other == null)
                return false;
            return this.GetIdentifier() == other.GetIdentifier();
        }
    }

    public enum ItemType
    {
        Crown,  // assign all tiles surrounding player
        Flag,   // assign 1 random tile to the player; won't select a tile that is the player's
        Lock,   // lock all of your tiles from being overwritten in the next turn
        Pillar, // assign all tiles along the direction the player is currently facing
        GroundSpike,    // make a tile inaccessible for 1 turn cycle
        SelfDestruct,   // set tiles surrounding player to unassigned

        Coin,   // gain 1 extra step
        Cage,   // skip a player's next turn
        BoulderSpike,   // return a player to their spawn point
        Bed,    // return back to own spawn point
        Door,   // teleport to a random spawn point
        WizardHat,  // swap places with another player
        Sliding,
        NIL = -1
    }
}
