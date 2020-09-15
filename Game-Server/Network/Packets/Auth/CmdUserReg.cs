using System;
using System.Collections.Generic;
using System.Text;

namespace Game_Server.Network
{
    public class CmdUserReg : InPacket
    {
        public readonly string Username;
        public readonly string Password;
        public readonly string Email;
        public readonly string StudentName;
        public readonly string Class;
        public readonly int Semester;
        public readonly int Year;

        public CmdUserReg(Packet packet) : base(packet)
        {
            Username = Reader.ReadUnicodeStatic(13).Trim();
            Password = Reader.ReadUnicodeStatic(40).Trim();
            Email = Reader.ReadUnicode().Trim();
            StudentName = Reader.ReadUnicode().Trim();
            Class = Reader.ReadUnicodeStatic(4).Trim();
            Semester = Reader.ReadInt32();
            Year = Reader.ReadInt32();
        }

    }
}
