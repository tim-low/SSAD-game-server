using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Game_Server.Network;
using Game_Server.Util;

namespace Game_Server.Model
{
    /// <summary>
    /// Representation of a Game Client in the Game Context
    /// </summary>
    public class Player : ISerializable, GameObject
    {
        /// <summary>
        /// The underlying Socket connection of the player
        /// This object won't be serialize to the client side
        /// </summary>
        public GameClient Client { get; private set; }

        /// <summary>
        /// The inventory item which the player currently possessed
        /// </summary>
        public List<InventoryItem> Items { get; private set; }

        /// <summary>
        /// Set initially by the game, to determine the Color of the <seealso cref="BoardTile">BoardTile</seealso>
        /// </summary>
        public int PlayerColor { get; set; }

        /// <summary>
        /// The BoardTile in which the player is currently on (mainly focus on the X, and Y axis)
        /// </summary>
        public BoardTile Position { get; set; }

        public BoardDirection FacingDirection { get; set; }

        public BoardTile OutTile { get; set; }

        /// <summary>
        /// The amount of move left by the player, this is refreshed each time a new cycle start
        /// </summary>
        public int MoveLeft { get; set; }

        public bool Answered { get; set; }

        public bool Acknowledge { get; set; }

        public ItemType Effect { get; set; }

        public int ExperienceGained { get; set; }

        public int Score { get; set; }
        /// <summary>
        /// This is used mostly for Slide All The Way To End effect, so that moving a player will move it all the way to the end instead
        /// </summary>

        /// <summary>
        /// Constructor to initialize the Player object in the Game Client
        /// </summary>
        /// <param name="client"></param>
        public Player(GameClient client)
        {
            this.Client = client;
            this.Items = new List<InventoryItem>();
            this.Answered = false;
            this.Acknowledge = false;
            FacingDirection = BoardDirection.UP;
            this.Effect = ItemType.NIL;
            this.Score = 0;
            this.ExperienceGained = 0;
        }

        /// <summary>
        /// Destructor to clear the list to remove some memory before GC come and clear it
        /// </summary>
        ~Player()
        {
            this.Items.Clear();
        }

        public void Receive(Reward reward)
        {
            Log.Debug("Reward: {0} - {1}", reward.Item.Item.ToString(), reward.Step);
            if (reward.Item.Item != ItemType.NIL)
            {
                Items.Add(reward.Item);
                Score += 100;
                ExperienceGained += 100;
            }
            MoveLeft = reward.Step;
        }

        public bool UseItem(ItemType identifier)
        {
            if (MoveLeft < 1)
                return false;
            MoveLeft -= 1; // Set to true regardless whether user use an item or not, this is to penalize them in case they trying to be funny
            List<InventoryItem> items = Items.Where(item => item.Item == identifier).ToList();
            if (items.Count() == 0)
                return false;
            return Items.Remove(items.First());
        }

        /// <summary>
        /// We will be sending this data over, 
        /// </summary>
        /// <param name="writer"></param>
        public void Serialize(SerializeWriter writer)
        {
            writer.WriteTextStatic(GetIdentifier(), 44); // Write Token (44)
            writer.WriteTextStatic(Client.Character.Name, 13);
            writer.Write(Position); // Write Position (3)
            writer.Write(MoveLeft); // Write MoveLeft (4)
            writer.Write(Items.Count); // Write Item Count (4)
            foreach(InventoryItem item in Items)
            {
                writer.Write(item); // Write Individual Item (0)
            }
        }

        /// <summary>
        /// Determine whether player turn have end
        /// </summary>
        /// <returns></returns>
        public bool HasTurnEnded()
        {
            return MoveLeft == 0;
        }

        public void ResetTurn()
        {
            Answered = false;
        }

        public string GetIdentifier()
        {
            return Client.Character.Token;
        }
    }
}
