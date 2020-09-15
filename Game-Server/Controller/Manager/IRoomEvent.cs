using Game_Server.Model;

namespace Game_Server.Controller.Manager
{
    public interface IRoomEvent
    {
        void OnRoomCreated(Lobby lobby, string roomId);
        void OnRoomDeleted(string roomId);
    }
}