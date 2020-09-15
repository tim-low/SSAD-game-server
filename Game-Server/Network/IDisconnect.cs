using System;
namespace Game_Server.Network
{
    /// <summary>
    /// IDisconnect will be implemented to all object that
    /// contain a list of GameClient, so that we can handle
    /// disconnection in the object while a GameClient dc.
    /// </summary>
    public interface IDisconnect
    {
        void Disconnect(GameClient client);
    }
}
