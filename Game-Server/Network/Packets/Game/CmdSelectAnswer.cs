using System;
namespace Game_Server.Network
{
    public class CmdSelectAnswer
    {
        public readonly string Token;
        public readonly int SelectedAnswer;

        public CmdSelectAnswer(Packet packet)
        {
            Token = packet.Reader.ReadUnicodeStatic(44);
            SelectedAnswer = packet.Reader.ReadInt32();
        }
    }
}
