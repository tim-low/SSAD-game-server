using Game_Server.Controller.Database.Tables;
using Game_Server.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Game_Server.Network
{
    public class GetUnlocksAck : OutPacket
    {
        public Inventory Inventory;
        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.GetUnlocksAck);
        }

        public override int ExpectedSize()
        {
            return base.ExpectedSize();
        }

        public override byte[] GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new SerializeWriter(ms))
                {
                    sw.Write(Inventory.Head);
                    sw.Write(Inventory.Shirt);
                    sw.Write(Inventory.Pant);
                    sw.Write(Inventory.Shoe);
                }
                return ms.ToArray();

            }
        }
    }
}
