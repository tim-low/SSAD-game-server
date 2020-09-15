using Game_Server.Model;
using Game_Server.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Game_Server.Network
{
    public class InitializeGameAck : OutPacket
    {
        public Player[] PlayerSequence;
        public byte GameTurn;

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.InitializeGameAck);
        }

        public override int ExpectedSize()
        {
            return 4 + (PlayerSequence.Length * (55+4)) + 4;
        }

        public override byte[] GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new SerializeWriter(ms))
                {
                    sw.Write(PlayerSequence.Length);
                    foreach(Player player in PlayerSequence)
                    {
                        sw.Write(player);
                        sw.Write(player.Client.Character.CharacterDb.HeadEqp);
                        sw.Write(player.Client.Character.CharacterDb.TopEqp);
                        sw.Write(player.Client.Character.CharacterDb.BottomEqp);
                        sw.Write(player.Client.Character.CharacterDb.ShoeEqp);
                    }
                    sw.Write(GameTurn);
                }
                return ms.ToArray();
            }
        }
    }
}
