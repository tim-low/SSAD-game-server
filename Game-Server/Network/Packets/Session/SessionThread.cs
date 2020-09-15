using Game_Server.Model;
using Game_Server.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game_Server.Network
{
    public class SessionThread
    {
        [Packet(Packets.CmdJoinSession)]
        public static void HandleJoinSession(Packet packet)
        {
            // Check if the sender is a teacher application user
            if(packet.Sender.IsTeacherClient())
            {
                // A teacher application user cannot join session as it is not processed to do so.
                packet.SendBackError(21);
                return;
            }
            // Read the data that was given to us by the client
            CmdJoinSession cmd = new CmdJoinSession(packet);
            Log.Info("Session: {0}", cmd.SessionCode);
            Room room;
            // Try and retrieve a WaitingRoom object
            if (ServerMain.Instance.Server.LobbyManager.Custom.RoomManager.Get(cmd.SessionCode, out room))
            {
                // Check if the room is currently in game
                if(room.IsInGame)
                {
                    packet.SendBackError(16);
                    return;
                } 
                else if(room.IsFull()) // Else check if the room is currently full
                {
                    packet.SendBackError(17);
                    return;
                }
                room.Join(packet.Sender);

                // Prepare the packet and broadcast to the user
                var ack1 = new JoinSessionAck()
                {
                    QuizName = room.Quiz.Name,
                    WaitingRoom = room
                };
                // Give client room details when they join
                packet.SendBack(ack1.CreatePacket());

                // Prepare the packet for the teacher client
                var ack2 = new TeacherPlayerJoinedSessionAck()
                {
                    StudentName = packet.Sender.Character.Name
                };
                room.Owner.Send(ack2.CreatePacket());

                // Prepare the packet for everybody in the room
                var ack3 = new ClientPlayerJoinedSessionAck()
                {
                    Character = packet.Sender.Character
                };
                // Everyone in the room should receive note of the new player except for the player itself sicne ack1 is given to them
                room.Broadcast(ack3.CreatePacket(), packet.Sender);
                return;
            }
            else
            {
                packet.SendBackError(21);
                return;
            }
        }

        [Packet(Packets.CmdLeaveSession)]
        public static void HandleLeaveSession(Packet packet)
        {
            CmdLeaveSession cmd = new CmdLeaveSession(packet);
            if(packet.Sender.Character.Status.GetState() == typeof(WaitingRoom))
            {
                WaitingRoom room = packet.Sender.Character.Status.GetObject<WaitingRoom>();
                room.Leave(packet.Sender);
                var ack3 = new LeaveSessionAck()
                {
                    Token = packet.Sender.GetIdentifier()
                };
                packet.SendBack(ack3.CreatePacket());


                var ack1 = new ClientPlayerLeftSessionAck()
                {
                    Token = packet.Sender.GetIdentifier()
                };
                room.Broadcast(ack1.CreatePacket());

                var ack2 = new TeacherPlayerLeftSessionAck()
                {
                    StudentName = packet.Sender.Character.Name
                };
                room.Owner.Send(ack2.CreatePacket());
                return;
            }
            else
            {
                packet.SendBackError(21);
                return;
            }
        }

        [Packet(Packets.CmdStartSession)]
        public static void HandleStartSession(Packet packet)
        {
            CmdStartSession cmd = new CmdStartSession(packet);
            Room room;
            bool success = ServerMain.Instance.Server.LobbyManager.Custom.RoomManager.Get(cmd.SessionCode, out room);
            #region ERROR_CHECKING
            if (!packet.Sender.IsTeacherClient())
            {
                packet.SendBackError(3);
                return;
            }
            else if (packet.Sender.Character.Permission == 0)
            {
                packet.SendBackError(4);
                return;
            }
            else if(packet.Sender.Character.Status.GetState() != typeof(WaitingRoom))
            {
                packet.SendBackError(11);
                return;
            }
            else if (packet.Sender.Character.Status.GetState() == typeof(WaitingRoom) && packet.Sender.Character.Status.GetObject<WaitingRoom>().Owner.GetIdentifier() != packet.Sender.GetIdentifier())
            {
                packet.SendBackError(21);
                return;
            }
            else if(!success)
            {
                packet.SendBackError(21);
                return;
            }
            #endregion

            WaitingRoom wroom = (WaitingRoom)room;
            Game[] games = wroom.StartSession();
            foreach(Game game in games)
            {
                game.Broadcast(new StartSessionAck()
                {
                    Success = true
                }.CreatePacket());
            }
            wroom.Owner.Send(new StartSessionAck()
            {
                Success = true
            }.CreatePacket());
            return;

        }

        [Packet(Packets.CmdCreateSession)]
        public static void HandleCreateSession(Packet packet)
        {
            if(!packet.Sender.IsTeacherClient())
            {
                packet.SendBackError(3);
                return;
            }
            if(packet.Sender.Character.Permission == 0)
            {
                packet.SendBackError(4);
                return;
            }
            CmdCreateSession cmd = new CmdCreateSession(packet);
            Room room;
            string code = ServerMain.Instance.Server.LobbyManager.Custom.RoomManager.Create(packet.Sender, cmd.QuizId, out room);
            packet.Sender.Character.Status.Update(room);
            var ack = new CreateSessionAck()
            {
                SessionCode = code
            };
            packet.SendBack(ack.CreatePacket());
            return;
        }

        [Packet(Packets.CmdCancelSession)]
        public static void HandleCancelSession(Packet packet)
        {
            CmdCancelSession cmd = new CmdCancelSession(packet);
            if (!packet.Sender.IsTeacherClient())
            {
                packet.SendBackError(3);
                return;
            }
            if (packet.Sender.Character.Permission == 0)
            {
                packet.SendBackError(4);
                return;
            }
            Room room;
            if(ServerMain.Instance.Server.LobbyManager.Custom.RoomManager.Get(cmd.SessionCode, out room))
            {
                if (room.Owner.GetIdentifier() == packet.Sender.GetIdentifier())
                {
                    ((WaitingRoom)room).Cancel();
                    var ackPkt = new CancelSessionAck().CreatePacket();
                    room.Broadcast(ackPkt);
                    room.Owner.Send(ackPkt);
                    room.Clients.Clear();
                }
                else
                {
                    packet.SendBackError(21);
                }
            }
            else
            {
                packet.SendBackError(15);
            }
            return;
        }
    }
}
