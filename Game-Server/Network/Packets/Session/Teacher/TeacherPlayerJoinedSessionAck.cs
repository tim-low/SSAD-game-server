using Game_Server.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Game_Server.Network
{
    public class TeacherPlayerJoinedSessionAck : OutPacket
    {
        public string StudentName;
        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.TeacherPlayerJoinedSessionAck);
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
                    sw.WriteText(StudentName, false);
                }
                return ms.ToArray();
            }
        }
    }
}
