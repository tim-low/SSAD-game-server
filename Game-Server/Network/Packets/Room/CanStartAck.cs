using System;
using System.IO;
using Game_Server.Model;
using Game_Server.Util;
using System.Numerics;

namespace Game_Server.Network
{
    //To notify the client whether the start button can be pressed by the host 
    class CanStartAck : OutPacket
    {
        public bool Success;

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.CanStartAck);
        }

        public override int ExpectedSize() => 1;

        public override byte[] GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new SerializeWriter(ms))
                {
                    sw.Write(Success);
                }
                return ms.ToArray();
            }
        }
    }
}
