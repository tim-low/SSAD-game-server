using System;
using System.Collections.Generic;
using System.Linq;
using Game_Server.Controller.Manager;
using Game_Server.Network;
using Game_Server.Util;

namespace Game_Server.Model
{
    public class Lobby : GameObject, IBroadcaster, IDisconnect
    {
        private Guid Guid;

        private List<GameClient> LobbyUsers;

        public RoomManager RoomManager { get; private set; }

        public int TopicId { get; private set; }

        public Lobby(IRoomEvent events, int topicId)
        {
            Guid = Guid.NewGuid();
            LobbyUsers = new List<GameClient>();
            RoomManager = new RoomManager(this);
            RoomManager.Subscribe(events);
            TopicId = topicId;
        }

        public string GetIdentifier()
        {
            return this.Guid.ToString();
        }

        public void Join(GameClient client)
        {
            if (LobbyUsers.Contains(client))
                return;
            LobbyUsers.Add(client);
            client.Character.Status.Update(this);
        }

        public void Leave(GameClient client)
        {
            if (LobbyUsers.Contains(client))
                if (LobbyUsers.Remove(client))
                    client.Character.Status.Update(null);
            return;
        }

        public void Broadcast(Packet packet, GameClient exclude = null)
        {
            foreach (GameClient lobbyuser in LobbyUsers)
            {
                if (exclude == null || exclude != lobbyuser)
                    lobbyuser.Send(packet);
            }
            return;
        }

        public void Broadcast(Packet packet, GameClient[] excludes)
        {
            foreach (GameClient lobbyuser in LobbyUsers)
            {
                if (!excludes.Contains(lobbyuser))
                    lobbyuser.Send(packet);
            }
            return;
        }

        public void Disconnect(GameClient client)
        {
            this.LobbyUsers.Remove(client);
            //this.GetPlayer().Remove(this.GetPlayer().SingleOrDefault(plyr => plyr.GetIdentifier() == client.User.Token));
            // need to figure out a way to slowly escalate up to GameServer and destroy the client
            return;
        }
    }
}
