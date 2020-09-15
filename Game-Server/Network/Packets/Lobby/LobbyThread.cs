using System;
using System.Collections.Generic;
using Game_Server.Model;
using Game_Server.Util;
using Game_Server.Manager;
using System.Threading;

namespace Game_Server.Network.Handler
{
    public class LobbyThread
    {
        [Packet(Packets.CmdCreateRoom)]
        public static void HandleCreateRoom(Packet packet)
        {
            CmdCreateRoom cmd = new CmdCreateRoom(packet);

            if (packet.Sender.Character?.Token != cmd.Token)
            {
                packet.SendBackError(12);
                return;
            }

            Character character = packet.Sender.Character;

            // User can only create room in lobby
            if (character?.Status?.GetState() != typeof(Lobby))
            {
                packet.SendBackError(11);
                return;
            }

            Lobby userLobby = character.Status.GetObject<Lobby>();

            Room createdRoom;
            userLobby.RoomManager.Create(
                packet.Sender,
                cmd.RoomName, 4, cmd.NumTurns, cmd.IsLocked, cmd.Password, out createdRoom);

            if (createdRoom == null)
            {
                packet.SendBackError(14);
                return;
            }

            createdRoom.Join(packet.Sender);

            // Send acks
            var ack1 = new CreateRoomAck()
            {
                Room = createdRoom
            };
            var ack2 = new RoomCreatedAck()
            {
                Room = createdRoom
            };

            // Broadcast to everyone in lobby that room has been created
           createdRoom.Parent.Broadcast(ack2.CreatePacket());

            // Give client room details
            packet.SendBack(ack1.CreatePacket());
        }

        [Packet(Packets.CmdJoinRoom)]
        public static void HandleJoinRoom(Packet packet)
        {
            CmdJoinRoom cmd = new CmdJoinRoom(packet);

            if (packet.Sender.Character?.Token != cmd.Token)
            {
                packet.SendBackError(12);
                return;
            }

            Character character = packet.Sender.Character;

            // User can only join room from lobby
            if (character?.Status?.GetState() != typeof(Lobby))
            {
                packet.SendBackError(11);
                return;
            }

            Lobby userLobby = character.Status.GetObject<Lobby>();

            Room joinedRoom;

            // Unable to find the room in RoomManager
            if (!userLobby.RoomManager.Get(cmd.RoomId, out joinedRoom))
            {
                packet.SendBackError(15);
                return;
            }

            if (joinedRoom.IsInGame)
            {
                packet.SendBackError(16);
                return;
            }

            // Room is full (max 4)
            if (joinedRoom.IsFull())
            {
                packet.SendBackError(17);
                return;
            }

            // Room is locked and client entered wrong password
            if (cmd.IsLocked && cmd.Password != joinedRoom.Password)
            {
                packet.SendBackError(18);
                return;
            }

            // Room is not locked or successfully entered password

            // Update status to that room.
            joinedRoom.Join(packet.Sender);

            var ack1 = new JoinRoomAck()
            {
                Room = joinedRoom
            };
            var ack2 = new UpdateRoomPlayerCountAck()
            {
                Room = joinedRoom
            };
            var ack3 = new NewPlayerInRoomAck()
            {
                Token = packet.Sender.Character.Token,
                Character = character
            };

            // Give client room details when they join
            packet.SendBack(ack1.CreatePacket());

            // Everyone in the lobby should receive note that the player count increased
            userLobby.Broadcast(ack2.CreatePacket());

            // Everyone in the room should receive note of the new player except for the player itself sicne ack1 is given to them
            joinedRoom.Broadcast(ack3.CreatePacket(), packet.Sender);
        }

        [Packet(Packets.CmdLeaveRoom)]
        public static void HandleLeaveRoom(Packet packet)
        {
            CmdLeaveRoom cmd = new CmdLeaveRoom(packet);

            if (packet.Sender.Character?.Token != cmd.Token)
            {
                packet.SendBackError(12);
                return;
            }

            Character character = packet.Sender.Character;

            // User can only leave from room
            if (character?.Status?.GetState() != typeof(Room))
            {
                packet.SendBackError(11);
                return;
            }

            Room room = character.Status.GetObject<Room>();

            // Update Status from Room to Lobby
            bool hasOwnerChange = room.Leave(packet.Sender);

            character.Unready();

            var ack1 = new LeaveRoomAck()
            {
                Lobby = room.Parent
            };
            var ack2 = new UpdateRoomPlayerCountAck()
            {
                Room = room
            };
            var ack3 = new PlayerHasLeftRoomAck()
            {
                Token = packet.Sender.Character?.Token,
                Character = character,
                HasOwnerChange = hasOwnerChange,
                Owner = room.Owner.GetIdentifier()
            };

            // Tell client if they successfully left room
            packet.SendBack(ack1.CreatePacket());

            room.Broadcast(ack3.CreatePacket());
            // Everyone in the lobby should receive note that the player count decreased

            packet.Sender.Character.Vec3 = new System.Numerics.Vector3(0, 0, 1);

            // 0.5 sleep then broadcast
            Thread.Sleep(500);

            packet.Sender.Character.Status.GetObject<IBroadcaster>().Broadcast(ack2.CreatePacket());

            // Everyone in the room should receive note of the player that left.
        }

        [Packet(Packets.CmdWorldSelect)]
        public static void HandleWorldSelect(Packet packet)
        {
            CmdWorldSelect cmd = new CmdWorldSelect(packet);

            if (packet.Sender.Character?.Token != cmd.Token)
            {
                packet.SendBackError(12);
                return;
            }

            Character character = packet.Sender.Character;

            // User can only create room in overworld; no status
            if (character?.Status?.GetState() == typeof(Room) || character?.Status?.GetState() == typeof(Lobby) || character?.Status?.GetState() == typeof(Game))
            {
                packet.SendBackError(11);
                return;
            }

            Lobby joinedLobby;
            ServerMain.Instance.Server.LobbyManager.Get(cmd.LobbyId, out joinedLobby);
            if (joinedLobby == null)
            {
                packet.SendBackError(19, cmd.LobbyId);
                return;
            }

            // Update client status, they are now in the lobby
            joinedLobby.Join(packet.Sender);

            var ack1 = new WorldSelectAck()
            {
                Lobby = joinedLobby
            };

            // Send client an array of rooms they sent.
            packet.SendBack(ack1.CreatePacket());
        }

        [Packet(Packets.CmdLeaveLobby)]
        public static void HandleLeaveLobby(Packet packet)
        {
            CmdLeaveLobby cmd = new CmdLeaveLobby(packet);

            if (packet.Sender.Character?.Token != cmd.Token)
            {
                packet.SendBackError(12);
                return;
            }

            Character character = packet.Sender.Character;

            // User can only leave from lobby
            if (character?.Status?.GetState() != typeof(Lobby))
            {
                packet.SendBackError(11);
                return;
            }

            character?.Status.GetObject<Lobby>().Leave(packet.Sender);

            var ack1 = new LeaveLobbyAck()
            {
                Success = true,
                Message = "Left lobby successfully"
            };

            packet.SendBack(ack1.CreatePacket());
        }
    }
}