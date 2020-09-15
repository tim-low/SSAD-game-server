using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Game_Server.Model.Misc;
using Game_Server.Network;
using Game_Server.Util;

namespace Game_Server.Model
{
    public class Room : GameObject, ISerializable, IBroadcaster, IDisconnect
    {
        /// <summary>
        /// Guid unique identifier for the Room
        /// </summary>
        private Guid Guid;

        public Lobby Parent { get; set; }
        /// <summary>
        /// Room Name that is displayed to the client (15 characters long)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Is the room password protected?
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// Determine how many client can be in the room
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// List of clients currently in room
        /// </summary>
        public List<GameClient> Clients { get; set; }

        /// <summary>
        /// Owner of room
        /// </summary>
        public GameClient Owner { get; set; }

        /// <summary>
        /// Whether or not the room is currently in the midst of a game
        /// </summary>
        public bool IsInGame { get; set; }

        /// <summary>
        /// Indicate the number of questions that will be tested for this quiz.
        /// </summary>
        public int NoOfQuestion { get; set; }

        /// <summary>
        /// Despawn Manager: Despatch Despawn Ack to clients
        /// </summary>
        private DespawnDespatcher EventDespatcher;

        /// <summary>
        /// The quiz that will be used once the game start
        /// </summary>
        public Quiz Quiz { get; set; }

        public Mutex Mutex { get; set; }

        public Room(int size = 4)
        {
            this.Guid = Guid.NewGuid();
            this.Clients = new List<GameClient>();
            this.Size = size;
            this.EventDespatcher = new DespawnDespatcher(this);
            this.Mutex = new Mutex(false, this.Guid.ToString());
        }

        public virtual string GetIdentifier()
        {
            return this.Guid.ToString();
        }

        public void Serialize(SerializeWriter writer)
        {
            writer.WriteTextStatic(Guid.ToString(), 36);
            // Write Room Name
            writer.WriteTextStatic(Name, 40);
            // Write IsRoomLocked
            writer.Write(IsLocked);
            // Write Num Of Client
            writer.Write(Clients.Count);
            // Write Room Size
            writer.Write(Size);

            // Write IsInGame
            writer.Write(IsInGame);
        }

        /// <summary>
        /// Add a client to the room.
        /// </summary>
        /// <param name="client"></param>
        public void AddClient(GameClient client)
        {
            this.Clients.Add(client);
        }

        /// <summary>
        /// Add an owner to the room.
        /// </summary>
        /// <param name="client"></param>
        public void AddOwner(GameClient client)
        {
            this.Owner = client;
            // Refactor outside into the thread instead
            //this.Clients.Add(client);
        }

        /// <summary>
        /// Removes a client from the room.
        /// If the removed client is the current owner, randomly generate a new owner from the remaining clients.
        /// </summary>
        /// <param name="client"></param>
        public bool RemoveClient(GameClient client)
        {
            if (this.Clients.Contains(client))
            {
                // If the client to be removed is the owner, assign a new owner.
                if (this.Owner.Equals(client))
                {
                    this.Clients.Remove(client);    // remove owner
                    if (this.Clients.Count() == 0)
                    {
                        EndGame();
                    }
                    else
                    {
                        Random rng = new Random();
                        int idx = rng.Next(Clients.Count);
                        this.Owner = Clients[idx];      // reassign owner by random index
                        return true;
                    }
                }
                else
                {
                    this.Clients.Remove(client);
                    if(this.Clients.Count() == 0 && client.GetIdentifier() == Owner.GetIdentifier())
                    {
                        EndGame();
                    }
                }
                return false;
            }
            return false;
            // TODO: What should I do if cannot find the client?
        }

        /// <summary>
        /// Whether the room has reached its max capacity of 4.
        /// </summary>
        /// <returns></returns>
        public virtual bool IsFull()
        {
            return (Clients.Count >= Size) ? true : false;
        }

        /// <summary>
        /// Returns whether or not everyone (unincluding the owner) is ready.
        /// </summary>
        /// <returns></returns>
        public virtual bool IsEveryoneReady()
        {
            foreach (GameClient client in Clients)
            {
                if (!client.Equals(Owner) && !client.Character.IsReady)
                {
                    return false;
                }
            }
            return true;
        }

        public void SetAllUnready()
        {
            foreach (GameClient client in Clients)
            {
                client.Character.Unready();
            }
        }

        /// <summary>
        /// Returns whether the room is able to start game
        /// </summary>
        /// <returns></returns>
        public bool CanGameStart()
        {
            return IsEveryoneReady();
        }

        public void StartGame(out Game game)
        {
            Mutex.WaitOne();
            IsInGame = true;
            ServerMain.Instance.Server.GameManager.Create(this, out game);
            Mutex.ReleaseMutex();
        }

        public void EndGame()
        {
            IsInGame = false;
            if(Clients.Count == 0)
            {
                this.Parent.RoomManager.RemoveRoom(this);
            }
        }

        public virtual void Join(GameClient client)
        {
            if (Clients.Contains(client))
            {
                client.Character.Status.Update(this);
                return;
            }
            Mutex.WaitOne();
            if (IsInGame == false && Clients.Count < Size)
            {
                this.Parent.Leave(client);
                this.AddClient(client);
                client.Character.Status.Update(this);
            }
            else
            {
                client.SendError("Game has already started.");
            }
            Mutex.ReleaseMutex();
            Log.Info("End Execute");
        }

        public virtual bool Leave(GameClient client, bool dc = false)
        {
            Mutex.WaitOne();
            bool value = false;
            if (Clients.Contains(client))
            {
                if (dc)
                {
                    value = this.RemoveClient(client);
                    //this.Parent.Join(client);
                }
                else
                {
                    Mutex.WaitOne();
                    if (!IsInGame)
                    {
                        value = RemoveClient(client);
                        this.Parent.Join(client);
                    }
                    Mutex.ReleaseMutex();
                }
            }
            Mutex.ReleaseMutex();
            return value;
        }

        #region PACKETS
        public void Broadcast(Packet packet, GameClient exclude = null)
        {
            foreach (GameClient c in Clients)
            {
                if(exclude == null || exclude != c)
                    c.Send(packet);
            }
        }

        public void Broadcast(Packet packet, GameClient[] excludes)
        {
            foreach (var client in Clients)
                if (!excludes.Contains(client))
                    client.Send(packet);
        }

        public virtual void Disconnect(GameClient client)
        {
            bool IsOwner = false;
            if(this.Owner == client)
            {
                IsOwner = true;
            }
            this.Leave(client, true);
            client.Character.Status.Update(null);
            Parent.Disconnect(client);
            // Enqueue all disconnect client
            if (IsInGame || !IsInGame)
            {
                var ack2 = new UpdateRoomPlayerCountAck()
                {
                    Room = this
                };
                var ack3 = new PlayerHasLeftRoomAck()
                {
                    Token = client.Character?.Token,
                    Character = client.Character,
                    HasOwnerChange = IsOwner,
                    Owner = this.Owner.GetIdentifier()
                };
                // Invoke only when the room is in Room State and not in Game state
                EventDespatcher.EnqueueRoom(ack3.CreatePacket(), client);
                this.Parent.Broadcast(ack2.CreatePacket());
                // Invoke the despatcher in 10s
                new Task(() => EventDespatcher.Invoke()).Start();
            }
        }
        #endregion
    }
}
