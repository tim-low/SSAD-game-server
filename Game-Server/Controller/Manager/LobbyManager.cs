using System;
using System.Collections.Generic;
using System.Linq;
using Game_Server.Model;

namespace Game_Server.Controller.Manager
{
    // am thinking of whether do we really need this class as a manager. cos I think if we were to make this class, maybe, just maybe we can hold a collection current connected client
    // to make sending broadcasting of packet more efficient - idk, (seek opinion)
    public class LobbyManager : IRoomEvent
    {
        private List<Lobby> Lobbies;

        // To ensure O(1) in term of time complexity
        private Dictionary<string, Lobby> Entries = new Dictionary<string, Lobby>();

        // Last lobby is the custom lobby for session
        public Lobby Custom
        {
            get
            {
                return Lobbies.Last();
            }
        }

        public LobbyManager()
        {
            Lobbies = new List<Lobby>();
            for(int i = 0; i < 7; i++)
            {
                Lobby lobby = new Lobby(this, i+1);
                Lobbies.Add(lobby);
            }
        }

        public bool Get(int topicId, out Lobby obj)
        {
            obj = Lobbies.SingleOrDefault(lobby => lobby.TopicId == topicId);
            return obj != null;
        }

        // is there a better name for this, i think can optimize also
        // tried SingleOrDefault but came out error
        // best way to optimized is to assign lobby id to room, make the lobby a parent of the room
        // With Dictionary, we will achieve O(1) timing. But the space complexity will be higher
        // Leave it as it is here for now
        public bool GetRoom(string roomIdentifier, out Room outRoom)
        {
            outRoom = null;
            Lobby lobby;
            var success = Entries.TryGetValue(roomIdentifier, out lobby);
            if (!success)
                return false;
            if (lobby.RoomManager.Get(roomIdentifier, out outRoom))
                return true;
            return false;
        }

        public void OnRoomCreated(Lobby lobby, string roomId)
        {
            Entries.Add(roomId, lobby);
        }

        public void OnRoomDeleted(string roomId)
        {
            Entries.Remove(roomId);
        }
    }
}
