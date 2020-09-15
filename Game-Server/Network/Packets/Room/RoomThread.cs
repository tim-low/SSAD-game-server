using System;
using System.Collections.Generic;
using Game_Server.Model;
using Game_Server.Util;
using Game_Server.Manager;

namespace Game_Server.Network.Handler
{
    public class RoomThread
    {
        // When I want to get list of clients and know who is the owner of a room
        // Not really sure what's the use case of this
        // I think initially this is send after a success ack for the client who join the room
        [Packet(Packets.CmdRoomDetails)]
        public static void HandleRoomDetails(Packet packet)
        {
            CmdRoomDetails pkt = new CmdRoomDetails(packet);

            #region ERROR_CHECKS
            if (packet.Sender.Character?.Token != pkt.Token)
            {
                packet.SendBackError(12);
                return;
            }

            Room requestedRoom;
            ServerMain.Instance.Server.LobbyManager.GetRoom(pkt.RoomId, out requestedRoom);

            if (requestedRoom == null)
            {
                packet.SendBackError(21);
                return;
            }
            #endregion

            // Send acks
            var ack1 = new RoomDetailsAck()
            {
                Room = requestedRoom
            };

            // Give client room details
            packet.Sender.Send(ack1.CreatePacket());
            return;
        }

        [Packet(Packets.CmdRoomPlayerMove)]
        public static void HandleRoomPlayerMove(Packet packet)
        {
            CmdRoomPlayerMove pkt = new CmdRoomPlayerMove(packet);

            #region ERROR_CHECKS
            if (packet.Sender.Character?.Token != pkt.Token)
            {
                packet.SendBackError(12);
                return;
            }

            Character character = packet.Sender.Character;

            // User can only move in room
            if (character?.Status?.GetState() != typeof(Room))
            {
                packet.SendBackError(11);
                return;
            }

            Room userRoom;
            if (!ServerMain.Instance.Server.LobbyManager.GetRoom(character.Status.GetIdentifier(), out userRoom))
            {
                packet.SendBackError(21);
                return;
            }
            #endregion

            // Update vec
            character.Vec3 = pkt.TargetPos;
            character.Direction = pkt.TargetPos - pkt.StartPos;
            var ack1 = new RoomPlayerMoveAck()
            {
                Token = pkt.Token,
                StartPos = pkt.StartPos,
                TargetPos = pkt.TargetPos
            };

            character.Status.GetObject<Room>().Broadcast(ack1.CreatePacket(), packet.Sender);
            return;
        }

        // didn't use RoomId from packet
        [Packet(Packets.CmdStartGame)]
        public static void HandleStartGame(Packet packet)
        {
            CmdStartGame pkt = new CmdStartGame(packet);

            #region ERROR_CHECKS
            if (packet.Sender.Character?.Token != pkt.Token)
            {
                packet.Sender.SendError(12);
                return;
            }

            Character character = packet.Sender.Character;

            // Can only indicate start game in room
            if (character?.Status?.GetState() != typeof(Room))
            {
                packet.SendBackError(11);
                return;
            }

            Room userRoom;
            if (!ServerMain.Instance.Server.LobbyManager.GetRoom(character.Status.GetIdentifier(), out userRoom))
            {
                packet.SendBackError(21);
                return;
            }

            Lobby userLobby;
            if (!ServerMain.Instance.Server.LobbyManager.Get(character.Status.GetObject<Room>().Parent.TopicId, out userLobby))
            {
                packet.SendBackError(21);
                return;
            }
            #endregion

            // If the one wanting to start the game is the OWNER
            if (packet.Sender.Equals(userRoom.Owner))
            {
                // Room not allowed to start game.
                if (!userRoom.CanGameStart())
                {
                    packet.SendBackError(23);
                    return;
                }
                // Room allowed to start game.
                else
                {
                    Game game;
                    // Ask game manager to create the room
                    userRoom.StartGame(out game);
                    var ack1 = new StartGameAck()
                    {
                        Success = true
                    };
                    packet.Sender.Character.Status.GetObject<Game>().Broadcast(ack1.CreatePacket());
                    var ack2 = new UpdateRoomStatusAck()
                    {
                        Room = userRoom
                    };
                    // Tell everyone in lobby that the room is currently in-game
                    ServerMain.Instance.Server.BroadcastLobby(userLobby.GetIdentifier(), ack2.CreatePacket());
                }
            }
            // If not owner, means it's CLIENT wants to wants to indicate they're ready.
            else
            {
                if (character.ToggleReady() == false)
                {
                    packet.SendBackError(24);
                    return;
                }
                var ack3 = new ReadyStatusAck()
                {
                    Token = pkt.Token,
                    IsReady = character.IsReady
                };
                // Send to other people in the room the new ready status of the character
                packet.Sender.Character.Status.GetObject<IBroadcaster>().Broadcast(ack3.CreatePacket());
                // If after this packet is sent, the game is allowed to start, let the owner know
                
                // I think we need to send every time in case someone unreadies
                var ack4 = new CanStartAck()
                {
                    Success = false
                };

                if (userRoom.CanGameStart())
                {
                    // Tell owner that the game can start now
                    ack4.Success = true;
                }
                userRoom.Owner.Send(ack4.CreatePacket());
            }
            return;
        }
    }
}