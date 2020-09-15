using Game_Server.Controller.Manager;
using Game_Server.Model;
using Game_Server.Network;
using Game_Server.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Game_Server.Manager
{
    /// <summary>
    /// This manager is a singleton and can be used by either GameServer or ApiServer, 
    /// depending on which require data for the games.
    /// </summary>
    public class GameManager : BaseManager<Game>
    {

        #region Instance Variables

        private Dictionary<string, Game> GameSessions;

        #endregion

        public GameManager()
        {
            GameSessions = new Dictionary<string, Game>();
            Log.Info($"Game Manager started running at {DateTime.Now.ToString()}");
        }

        public string Create(Room r, out Game game)
        {
            game = GameFactory.CreateGame(r);
            GameSessions.Add(game.GetIdentifier(), game);
            return game.GetIdentifier();
        }

        /// <summary>
        /// Teacher Session Game - GameSession don't need to keep track, so that GarbageCollector can be activated once no client is pointing a reference
        /// to the Game object.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        public bool Create(WaitingRoom r, out Game[] game)
        {
            game = GameFactory.CreateGame(r);
            string prefix = r.GetIdentifier();
            int i = 1;
            var success = game.Where(x => x == null).Count() == 0;
            if (!success)
                return false;
            return success;
        }

        public void Remove(Game game)
        {
            if(GameSessions.ContainsKey(game.GetIdentifier()))
                GameSessions.Remove(game.GetIdentifier());
        }

        public bool Get(string identifier, out Game obj)
        {
            return GameSessions.TryGetValue(identifier, out obj);
        }
    }
}
