using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Game_Server.Network;
using Game_Server.Util;

namespace Game_Server.Model
{
    public class PlayerManager
    {
        // Synchronise the list
        public SynchronizeList<Player> PlayerList;
        public Queue<Player> Sequence;

        public PlayerManager()
        {
            PlayerList = new SynchronizeList<Player>();
            Sequence = new Queue<Player>();

        }

        public void Add(GameClient client)
        {
            this.PlayerList.Add(new Player(client));
        }

        public void Leave(GameClient client)
        {
            this.PlayerList.Remove(GetPlayer(client.Character.Token));
        }

        private Player GetPlayer(string token)
        {
            return this.PlayerList.Get(token);
        }

        public void Next()
        {
            var player = Sequence.Dequeue();
            while (Sequence.Count > 0 && !this.PlayerList.Contains(Sequence.Peek()))
            {
                Sequence.Dequeue();
            }
        }

        public bool IfThereStillPlayer()
        {
            return PlayerList.Count > 0;
        }

        private void NewCycle()
        {
            List<Player> temp = PlayerList.Randomize();
            foreach (Player p in temp)
            {
                Sequence.Enqueue(p);
            }
        }

        public void Setup()
        {
            NewCycle();
            ResetTurn();
        }

        public void PlayerAcknowledge(GameClient client)
        {
            if(PlayerList.Get(client.GetIdentifier()).Acknowledge == false)
                PlayerList.Get(client.GetIdentifier()).Acknowledge = true;
        }

        public bool CanSendInitializeAck()
        {
            return PlayerList.GetAll().Where(plyr => plyr.Acknowledge == false).Count() == 0;
        }

        public List<Player> GetPlayers()
        {
            return PlayerList.GetAll();
        }

        public List<Player> GetSequence()
        {
            return Sequence.ToList();
        }

        public Player GetCurrentPlayer()
        {
            return Sequence.Peek();
        }

        public void PrintPlayerPos()
        {
            foreach(Player p in PlayerList.GetAll())
            {
                Log.Debug("{0} ({1}, {2})", p.Client.Character.Name, p.Position.X, p.Position.Y);
            }
        }

        public Player MovePlayer(BoardTile tile, BoardDirection direction)
        {
            var curPlayer = GetCurrentPlayer();
            var oldPos = curPlayer.Position;
            // curPlayer.Position = tile;
            var othPlayer = PlayerList.GetAll().SingleOrDefault(plyr => plyr.Position.X == tile.X && plyr.Position.Y == tile.Y);
            if (othPlayer != null)
            {
                othPlayer.Position = oldPos;
                othPlayer.FacingDirection = Utilities.Opposite(direction);
            }
            curPlayer.Position = tile;
            curPlayer.FacingDirection = direction;
            curPlayer.MoveLeft -= 1;
            return curPlayer;
        }

        public Player GetPlayer(int playerIndex)
        {
            return Sequence.ToList().ElementAt(playerIndex);
        }

        public Player GetPlayer(GameClient client)
        {
            return PlayerList.Get(client.GetIdentifier());
        }

        public int PlayerCount()
        {
            return PlayerList.Count;
        }

        public bool CanSendCycleAck()
        {

            int playerAnswered = PlayerList.GetAll().Where(plyr => plyr.Answered == false).Count();
            bool canStartCycle = playerAnswered == 0;
            Log.Info("Player Not Answered: {0}, Can Start: {1}", playerAnswered, canStartCycle);
            return canStartCycle;
        }

        public void ResetTurn()
        {
           foreach(Player p in PlayerList.GetAll())
            {
                p.ResetTurn();
            }
        }

        public bool IsCycleStillOngoing()
        {
            return Sequence.Count > 0;
        }

        public void Clear()
        {
            this.PlayerList.Clear();
            this.Sequence.Clear();
        }

    }
}
