using System;
using System.Linq;
using Game_Server.Model;
using Game_Server.Util;

namespace Game_Server.Network
{
    public class GameThread
    {
        /// <summary>
        /// Thread to handle all incoming CmdInitializeGame packet
        /// Logic to shuffle the players need some fixing
        /// </summary>
        /// <param name="packet"></param>
        [Packet(Packets.CmdInitializeGame)]
        public static void HandleInitializeGame(Packet packet)
        {
            CmdInitializeGame pkt = new CmdInitializeGame(packet);
            if (packet.Sender.Character?.Token == pkt.Token)
            {
                Character character = packet.Sender.Character;
                if(character?.Status.GetState() != typeof(Game))
                {
                    // Shouldn't send us this packet if the character is not in game
                    packet.SendBackError(11);
                    return;
                }
                Game game = character.Status.GetObject<Game>();
                game.AcknowlegeInitialized(packet.Sender);
                // We can use the pkt.Token
                // ServerMain.Instance.Server.GameManager.Find()
            }
            else
            {
                packet.SendBackError(12);
                // Invalid packet send by the user with the wrong token
            }
            return;
        }

        [Packet(Packets.CmdTurnPlayerMove)]
        public static void HandlePlayerMove(Packet packet)
        {
            CmdTurnPlayerMove playerMovePkt = new CmdTurnPlayerMove(packet);
            if(playerMovePkt.Token == packet.Sender.Character.Token)
            {
                Character character = packet.Sender.Character;
                if (character?.Status.GetState() != typeof(Game))
                {
                    // Shouldn't send us this packet if the character is not in game
                    packet.SendBackError(11);
                    return;
                }
                Game game = character.Status.GetObject<Game>();
                bool success;
                Player player = game.MovePlayer(packet.Sender, playerMovePkt.Direction, out success);
                if (player == null)
                {
                    packet.SendBackError(13);
                    return;
                }
                var ack = new TurnPlayerMoveAck()
                {
                    IsLegalMove = success,
                    Token = player.GetIdentifier(),
                    NewPos = player.Position,
                    Direction = playerMovePkt.Direction
                };
                game.Broadcast(ack.CreatePacket());
            }
            else
            {
                packet.SendBackError(12);
            }
            return;
        }

        [Packet(Packets.CmdTurnPlayerUseItem)]
        public static void HandlePlayerItemUsage(Packet packet)
        {
            CmdTurnPlayerUseItem itemPkt = new CmdTurnPlayerUseItem(packet);
            Character character = packet.Sender.Character;
            if (character?.Status.GetState() != typeof(Game))
            {
                // Shouldn't send us this packet if the character is not in game
                packet.SendBackError(11);
                return;
            }
            Game game = character.Status.GetObject<Game>();
            var player = game.GetCurrentPlayer();
            if (player == null || player.GetIdentifier() != packet.Sender.Character?.Token)
            {
                packet.SendBackError(13);
                return;
            }
            BoardTile tile;
            if (game.HandleUseItem(itemPkt, out tile))
            {
                // Broadcast item usage to all client
                // what should be the state?
                var ack = new TurnPlayerUseItemAck()
                {
                    Caster = packet.Sender.Character?.Token,
                    Victim = itemPkt.Victim,
                    Item = itemPkt.Item,
                    Tile = tile
                };
                game.Broadcast(ack.CreatePacket());
            }
            return;
        }

        [Packet(Packets.CmdSelectAnswer)]
        public static void HandleSelectAnswer(Packet packet)
        {
            CmdSelectAnswer selectAnswerPkt = new CmdSelectAnswer(packet);
            if (selectAnswerPkt.Token == packet.Sender.Character?.Token)
            {
                Character character = packet.Sender.Character;
                if (character?.Status.GetState() != typeof(Game))
                {
                    // Shouldn't send us this packet if the character is not in game
                    packet.SendBackError(11);
                    return;
                }
                Game game = character.Status.GetObject<Game>();
                game.Answer(packet.Sender, selectAnswerPkt.SelectedAnswer);
                    // Check if all user have answered
                if(game.HasAllAnswered())
                {
                    game.QuizTimeUpOrAnswered();
                }
            }
            else
            {
                packet.SendBackError(12);
            }
            return;
        }

        [Packet(Packets.CmdEndTurn)]
        public static void HandleEndTurn(Packet packet)
        {
            Character character = packet.Sender.Character;
            if (character?.Status.GetState() != typeof(Game))
            {
                // Shouldn't send us this packet if the character is not in game
                packet.SendBackError(11);
                return;
            }
            Game game = character.Status.GetObject<Game>();
            if(packet.Sender.Character.Token == game.GetCurrentPlayer().GetIdentifier())
            {
                game.EndPlayerTurn();
            }
            return;
        }
    }
}
