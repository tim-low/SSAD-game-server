using Game_Server.Network;
using Game_Server.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game_Server.Model
{
    /// <summary>
    /// The WaitingRoom for a Quiz Session opened by the Teacher
    /// </summary>
    public class WaitingRoom : Room
    {
        public string SessionCode { get; set; }

        public WaitingRoom() : base(-1)
        {
            this.SessionCode = Utilities.GenerateCode(3);
        }
        /// <summary>
        /// A WatingRoom that is created by the teacher is never full, all user can join in
        /// </summary>
        /// <returns></returns>
        public override bool IsFull()
        {
            if (Size == -1)
                return false;
            else
                return Clients.Count() >= Size;
        }

        /// <summary>
        /// The users in a WaitingRoom will always be ready, the teacher can start whenever they want.
        /// </summary>
        /// <returns></returns>
        public override bool IsEveryoneReady()
        {
            return true;
        }

        public Game[] StartSession()
        {
            Game[] games;
            ServerMain.Instance.Server.GameManager.Create(this, out games);
            return games;
        }

        public override void Join(GameClient client)
        {
            if (Clients.Contains(client))
            {
                client.Character.Status.Update(this);
                return;
            }
            Log.Info("Waiting on thread.");
            Mutex.WaitOne();
            Log.Info("Execute");
            if (IsInGame == false)
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

        public override bool Leave(GameClient client, bool dc = false)
        {
            bool value = false;
            if (Clients.Contains(client))
            {
                if (dc)
                {
                    value = this.RemoveClient(client);
                }
                else
                {
                    Mutex.WaitOne();
                    if (!IsInGame)
                    {
                        value = RemoveClient(client);
                        client.Character.Status.Update(null);
                    }
                    Mutex.ReleaseMutex();
                }
            }
            return value;
        }

        public override string GetIdentifier()
        {
            return SessionCode;
        }

        public override void Disconnect(GameClient client)
        {
            this.Leave(client, true);
            client.Character.Status.Update(null);
            Parent.Disconnect(client);
            // Enqueue all disconnect client
            if (!IsInGame)
            {
                var ack2 = new ClientPlayerLeftSessionAck()
                {
                    Token = client.GetIdentifier()
                };
                this.Broadcast(ack2.CreatePacket(), client);
            }
        }

        public void Cancel()
        {
            foreach(var client in Clients)
            {
                Mutex.WaitOne();
                if (!IsInGame)
                {
                    client.Character.Status.Update(null);
                }
                Mutex.ReleaseMutex();
            }
            this.Owner.Character.Status.Update(null);
        }

    }
}
