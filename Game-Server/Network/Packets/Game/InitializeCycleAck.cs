using System;
using System.Collections.Generic;
using System.IO;
using Game_Server.Model;
using Game_Server.Util;

namespace Game_Server.Network
{
    public class InitializeCycleAck : OutPacket
    {

        public List<Player> PlayerSequence;

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.InitializeCycleAck);
        }


        public override int ExpectedSize()
        {
            return 0;
        }

        public override byte[] GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new SerializeWriter(ms))
                {
                    sw.Write(PlayerSequence.Count);
                    foreach(Player p in PlayerSequence)
                    {
                        sw.Write(p);
                    }
                }
                return ms.ToArray();
            }
        }

    }
}
