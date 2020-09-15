using System;
using System.Collections.Generic;
using System.Linq;
using Game_Server.Model;
using Game_Server.Network;

namespace Game_Server.Controller.Manager
{
    public class RoomManager : BaseManager<Room>
    {
        private Lobby Owner;
        private List<Room> Rooms;
        private IRoomEvent Observer;

        public RoomManager(Lobby lobby)
        {
            Rooms = new List<Room>();
            Owner = lobby;
        }

        public void Subscribe(IRoomEvent subscriber)
        {
            Observer = subscriber;
        }

        public List<Room> GetRooms()
        {
            return Rooms;
        }

        public void RemoveRoom(Room room)
        {
            Rooms.Remove(room);
            Observer.OnRoomDeleted(room.GetIdentifier());
        }

        public string Create(GameClient owner, string name, int size, int numQuestions, bool isLocked, string password, out Room room)
        {
            room = new Room(size);
            room.Parent = Owner;
            room.AddOwner(owner);
            room.Name = name;
            room.NoOfQuestion = numQuestions;
            room.IsLocked = isLocked;
            room.Password = password;
            Quiz quiz = new Quiz()
            {
                Id = Int32.MaxValue,
                Name = "Random",
                Questions = ServerMain.Instance.Database.GetQuestions(Owner.TopicId).ToList()
            };
            room.Quiz = quiz;
            Rooms.Add(room);
            Observer.OnRoomCreated(room.Parent, room.GetIdentifier());
            return room.GetIdentifier();
        }

        public string Create(GameClient owner, int quizId, out Room room)
        {
            room = new WaitingRoom();
            room.Parent = Owner;
            room.IsInGame = false;
            room.AddOwner(owner);
            Quiz quiz = new Quiz();
            quiz.FromEntity(ServerMain.Instance.Database.GetQuiz(quizId));
            room.Quiz = quiz;
            room.IsLocked = false;
            room.NoOfQuestion = -1;
            Rooms.Add(room);
            Observer.OnRoomCreated(room.Parent, room.GetIdentifier());
            return room.GetIdentifier();
        }

        public bool Get(string identifier, out Room obj)
        {
            obj = Rooms.SingleOrDefault(room => room.GetIdentifier() == identifier);
            return obj != null;
        }
    }
}
