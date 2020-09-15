using Game_Server.Model;
using Game_Server.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Game_Server.Network
{
    public class ClientPlayerJoinedSessionAck : OutPacket
    {
        public Character Character;
        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.ClientPlayerJoinedSessionAck);
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
                    sw.WriteTextStatic(Character.Token, 44);
                    sw.WriteTextStatic(Character.Name, 13);
                    sw.Write(Character.CharacterDb.HeadEqp);
                }
                return ms.ToArray();
            }
        }
    }
}
