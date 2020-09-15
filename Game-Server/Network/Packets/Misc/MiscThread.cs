using Game_Server.Controller.Database.Tables;
using Game_Server.Model;
using Game_Server.Model.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using Game_Server.Util;

namespace Game_Server.Network
{
    public class MiscThread
    {
        [Packet(Packets.CmdChatMsg)]
        public static void OnHandleChatMessage(Packet packet)
        {
            CmdChatMessage msgPkt = new CmdChatMessage(packet);
            MessageType type = msgPkt.Message.Type;
            var ack = new ChatMessageAnswer()
            {
                Message = msgPkt.Message
            };
            switch (type)
            {
                case MessageType.GAME:
                case MessageType.ROOM:
                case MessageType.LOBBY:
                    packet.Sender.Character.Status.GetObject<IBroadcaster>().Broadcast(ack.CreatePacket());
                    break;
                case MessageType.WORLD:
                    ServerMain.Instance.Server.Broadcast(ack.CreatePacket());
                    break;
            }
            return;
        }

        [Packet(Packets.CmdPlayerStats)]
        public static void OnHandlePlayerStats(Packet packet)
        {
            CmdPlayerStats playerStats = new CmdPlayerStats(packet);
            // Send the updated version over to client
            var ack = new PlayerStatsAck()
            {
                LeaderboardStats = packet.Sender.Character.Statistic,
                Experience = packet.Sender.Character.Experience,
                GameCount = packet.Sender.Character.GameCount
            };
            packet.SendBack(ack.CreatePacket());
            return; 
        }

        [Packet(Packets.CmdSendKaomoji)]
        public static void OnHandleKaomoji(Packet packet)
        {
            CmdSendKaomoji cmd = new CmdSendKaomoji(packet);
            var status = packet.Sender.Character.Status;
            var ack = new KaomojiAck()
            {
                Token = packet.Sender.Character.Token,
                KaomojiId = cmd.KaomojiId
            };
            if(status.GetState() == typeof(Game) || status.GetState() == typeof(Room) || status.GetState() == typeof(WaitingRoom))
            {
                // KIV - need to run the client-server integration to actually test
                // I realise it might be possible to change a instance of an object via reference
                status.GetObject<IBroadcaster>().Broadcast(ack.CreatePacket(), packet.Sender);
            }
            else
            {
                packet.SendBackError(20);
            }
            return;
        }

        [Packet(Packets.CmdGetUnlocks)]
        public static void OnHandleGetInventory(Packet packet)
        {
            string token = packet.Reader.ReadUnicodeStatic(44);
            if (packet.Sender.Character == null)
            {
                packet.SendBackError(21);
                return;
            }
            Inventory entry = packet.Sender.Character.CharacterDb.Account.Inventory;//ServerMain.Instance.Database.GetInventory(packet.Sender.User.Uid.ToString());
            var ack = new GetUnlocksAck()
            {
                Inventory = entry
            };
            packet.SendBack(ack.CreatePacket());
            return;
        }

        [Packet(Packets.CmdGetChar)]
        public static void OnGetAvatar(Packet packet)
        {
            var character = packet.Sender.Character;
            var ack = new GetCharAck()
            {
                Head = character.CharacterDb.HeadEqp,
                Shirt = character.CharacterDb.TopEqp,
                Pant = character.CharacterDb.BottomEqp,
                Shoe = character.CharacterDb.ShoeEqp,
                ChestCount = character.CharacterDb.ChestCount
            };
            packet.SendBack(ack.CreatePacket());
            return;
        }

        [Packet(Packets.CmdLeaderboard)]
        public static void OnLoadRanking(Packet packet)
        {
            CmdLeaderboard cmd = new CmdLeaderboard(packet);
            int rank;
            bool last;
            List<Ranking> rankings = ServerMain.Instance.Database.GetLeaderboard(packet.Sender.Character.Name, cmd.LifeCycleStage, cmd.PageNum, cmd.PageSize, out rank, out last);
            var ack = new LeaderboardAck()
            {
                OwnRanking = rank,
                IsLastPage = last,
                Entries = rankings.ToArray()
            };
            packet.SendBack(ack.CreatePacket());
            return;
        }

        [Packet(Packets.CmdOpenChest)]
        public static void OnOpenChest(Packet packet)
        {
            CmdOpenChest cmd = new CmdOpenChest(packet);

            if (packet.Sender.Character.Token != cmd.Token)
            {
                packet.SendBackError(12);
                return;
            }

            Model.Character character = packet.Sender.Character;

            if(character.CharacterDb.ChestCount > 0)
            {
                Random rnd = new Random();
                int attributeNumber = rnd.Next(0, 4);
                int unlockedItemNumber = rnd.Next(0, 8);
                var entry = packet.Sender.Character.CharacterDb.Account.Inventory;
                switch (attributeNumber)
                {
                    case 0:
                        entry.Head = Utilities.AddAttributeItem(entry.Head, unlockedItemNumber);
                        break;
                    case 1:
                        entry.Shirt = Utilities.AddAttributeItem(entry.Shirt, unlockedItemNumber);
                        break;
                    case 2:
                        entry.Pant = Utilities.AddAttributeItem(entry.Pant, unlockedItemNumber);
                        break;
                    case 3:
                        entry.Shoe = Utilities.AddAttributeItem(entry.Shoe, unlockedItemNumber);
                        break;
                }
                character.CharacterDb.ChestCount--;
                var ack = new GetChestRewardAck
                {
                    AttributeNumber = attributeNumber,
                    ItemNumber = unlockedItemNumber
                };
                packet.SendBack(ack.CreatePacket());
            }
            else
            {
                packet.SendBackError(22);
            }
        }

        [Packet(Packets.CmdGloryboard)]
        public static void OnLoadHallOfFame(Packet packet)
        {
            CmdGloryboard cmd = new CmdGloryboard(packet);
            bool isLastPage;
            List<Gloryboard> entries = ServerMain.Instance.Database.GetGloryboard(cmd.PageNum, cmd.EntriesNum, out isLastPage);
            packet.SendBack(new GloryboardAck()
            {
                IsLastPage = isLastPage,
                Entries = entries
            }.CreatePacket());
            return;
        }

        [Packet(Packets.NullPingAck)]
        public static void UpdateClientAck(Packet packet)
        {
            packet.Sender.Acknowledge();
            return;
        }

        [Packet(Packets.CmdGetNotices)]
        public static void GetNotices(Packet packet)
        {
            var ack = new GetNoticesAck()
            {
                Notices = ServerMain.Instance.Annoucement.Keys.ToArray()
            };
            packet.SendBack(ack.CreatePacket());
            return;
        }

        [Packet(Packets.CmdNoticeData)]
        public static void GetNoticesData(Packet packet)
        {
            CmdNoticeData cmd = new CmdNoticeData(packet);
            Annoucement[] annoucements = new Annoucement[cmd.AnnoucementIndexes.Length];
            for(int i = 0; i < cmd.AnnoucementIndexes.Length; i++)
            {
                annoucements[i] = ServerMain.Instance.Annoucement[cmd.AnnoucementIndexes[i]];
            }
            var ack = new NoticeDataAck()
            {
                Annoucements = annoucements
            };
            packet.SendBack(ack.CreatePacket());
            return;
        }
    }
}
