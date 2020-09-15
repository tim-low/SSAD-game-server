using Game_Server.Network;
using Game_Server.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Game_Server.Model.Misc
{
    public class DespawnDespatcher
    {
        private ConcurrentQueue<Packet> DespawnPackets;
        private ConcurrentQueue<GameClient> Objects;
        private IBroadcaster Broadcaster;

        public bool IsInvoking { get; private set; }
        
        public DespawnDespatcher(IBroadcaster _speaker)
        {
            DespawnPackets = new ConcurrentQueue<Packet>();
            Objects = new ConcurrentQueue<GameClient>();
            Broadcaster = _speaker;
            IsInvoking = false;
        }

        public void Enqueue(GameClient item)
        {
            DespawnPackets.Enqueue(new DespawnPlayerAck()
            {
                Token = item.GetIdentifier()
            }.CreatePacket());
            Objects.Enqueue(item);
        }

        public void EnqueueRoom(Packet packet, GameClient item)
        {
            DespawnPackets.Enqueue(packet);
            Objects.Enqueue(item);
        }


        public GameClient[] Excludes()
        {
            return Objects.ToArray();
        }

        /// <summary>
        /// Schedule a broadcasting of despawn ack to all clients
        /// </summary>
        /// <returns></returns>
        public async Task Invoke()
        {
            if (IsInvoking == true)
                return;
            IsInvoking = true;
            await Task.Delay(Constant.EVENT_TRIGGER_MILLIS);
            Packet packet;
            // Once despawn
            while(DespawnPackets.TryDequeue(out packet))
            {
                Broadcaster.Broadcast(packet, Excludes());
            }
            IsInvoking = false;
        }




    }
}
