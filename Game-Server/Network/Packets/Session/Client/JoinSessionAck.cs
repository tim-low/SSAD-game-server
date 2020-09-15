using Game_Server.Model;
using Game_Server.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Game_Server.Network
{
    public class JoinSessionAck : OutPacket
    {
        public string QuizName;
        public Room WaitingRoom;
        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.JoinSessionAck);
        }

        public override int ExpectedSize()
        {
            return 1;
        }

        public override byte[] GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new SerializeWriter(ms))
                {
                    sw.WriteText(QuizName, false);
                    sw.Write(WaitingRoom.Clients.Count);
                    foreach(var client in WaitingRoom.Clients)
                    {
                        sw.WriteTextStatic(client.Character.Token, 44);
                        sw.WriteTextStatic(client.Character.Name, 13);
                        sw.Write(client.Character.CharacterDb.HeadEqp);
                    }
                }
                return ms.ToArray();
            }
        }
    }
}
