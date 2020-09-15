using Game_Server.Controller.Database.Tables;

using Game_Server.Util;
using System.Collections.Generic;
using System.IO;

namespace Game_Server.Network
{
    public class GloryboardAck : OutPacket
    {
        public List<Gloryboard> Entries;
        public bool IsLastPage;

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.GloryboardAck);
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
                    sw.Write(IsLastPage);
                    sw.Write(Entries.Count);
                    foreach(var entry in Entries)
                    {
                        sw.Write(entry);
                    }
                }
                return ms.ToArray();
            }
        }
    }
}
