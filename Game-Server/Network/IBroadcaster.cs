using System;
using System.Collections.Generic;
using System.Text;

namespace Game_Server.Network
{
    /// <summary>
    /// IBroadcaster is an interface that an object can implement
    /// if the object is holding onto a list of GameClient and you
    /// wish to be able to send packets to all the clients in the list
    /// </summary>
    public interface IBroadcaster
    {
        void Broadcast(Packet packet, GameClient exclude = null);
        void Broadcast(Packet packet, GameClient[] excludes);
    }
}
