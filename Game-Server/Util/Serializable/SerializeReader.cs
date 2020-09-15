﻿using System;
using System.IO;
using System.Numerics;
using System.Text;

namespace Game_Server.Util
{
    public class SerializeReader : BinaryReader
    {
        public SerializeReader(Stream stream) : base(stream, Encoding.Unicode)
        {
        }

        public SerializeReader(Stream stream, Encoding encoding) : base(stream, encoding)
        {
        }

        public string ReadUnicode()
        {
            var sb = new StringBuilder();
            char val;
            do
            {
                val = ReadChar();
                if (val > 0)
                    sb.Append(val);
            } while (val > 0);
            return sb.ToString();
        }

        public Vector3 ReadVec3()
        {
            var vec = new Vector3();
            vec.X = ReadSingle();
            vec.Y = ReadSingle();
            vec.Z = ReadSingle();
            return vec;
        }

        public string ReadUnicodeStatic(int maxLength)
        {
            var buf = ReadBytes(maxLength * 2);
            var str = Encoding.Unicode.GetString(buf);

            if (str.Contains("\0"))
                str = str.Substring(0, str.IndexOf('\0'));

            return str;
        }

        public string ReadUnicodePrefixed()
        {
            var length = ReadUInt16();
            return ReadUnicodeStatic(length);
        }

        public string ReadAscii()
        {
            var sb = new StringBuilder();
            byte val;
            do
            {
                val = ReadByte();
                sb.Append((char)val);
            } while (val > 0);
            return sb.ToString();
        }

        public string ReadAsciiStatic(int maxLength)
        {
            var buf = ReadBytes(maxLength);
            var str = Encoding.ASCII.GetString(buf);

            if (str.Contains("\0"))
                str = str.Substring(0, str.IndexOf('\0'));

            return str;
        }
    }
}
