using System;
using System.IO;
using Game_Server.Model;
using Game_Server.Util;
using System.Numerics;

namespace Game_Server.Network
{
    /// <summary>
    /// Sent to everyone in the room when the game starts successfully to alert them to change scene.
    /// </summary>
    class StartGameAck : OutPacket
    {
        public bool Success;

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.StartGameAck);
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
