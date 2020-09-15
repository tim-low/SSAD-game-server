using System;
using System.IO;
using Game_Server.Model;
using Game_Server.Util;

namespace Game_Server.Network
{
    // Sent to player when they leave room
    class LeaveLobbyAck : OutPacket
    {
        public bool Success;
        public string Message;

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.LeaveLobbyAck);
        }

        public override int ExpectedSize() => 45;

        public override byte[] GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new SerializeWriter(ms))
                {
                    sw.Write(Success);
                    sw.WriteTextStatic(Message, 44);
                }
                return ms.ToArray();
            }
        }
    }
}
