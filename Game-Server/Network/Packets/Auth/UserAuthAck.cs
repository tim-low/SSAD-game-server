using System;
using System.IO;
using Game_Server.Util;

namespace Game_Server.Network.Auth
{
    public class UserAuthAck : OutPacket
    {
        /// <summary>
        /// Message will be the token if Success = true else it will output the message
        /// </summary>
        public string Token;

        public override Packet CreatePacket()
        {
            return base.CreatePacket(Packets.UserAuthAck);
        }

        public override int ExpectedSize() => 44; // 1 for boolean, 44 for the string, 1 for boolean
            
        public override byte[] GetBytes()
        {
            using(var ms = new MemoryStream())
            {
                using(var sw = new SerializeWriter(ms))
                {
                    sw.WriteTextStatic(Token, 44);
                }
                return ms.ToArray();
            }
        }
    }
}
