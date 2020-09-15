using Game_Server.Network;
using Game_Server.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace LoadTestingRunner
{
    public class CmdLogin : OutPacket
    {
        public string Username;
        public string Password;

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.CmdUserAuth);
        }

        public override int ExpectedSize()
        {
            return 53;
        }

        public override byte[] GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new SerializeWriter(ms))
                {
                    sw.WriteTextStatic(Username, 13);
                    sw.WriteTextStatic(Utilities.Sha1Sum2(Password), 40);
                }
                return ms.ToArray();
            }
        }
    }
}
