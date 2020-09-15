using System.Text;
using System.Security.Cryptography;
using System;

namespace LoadTestingRunner
{
    public class Utilities
    {
        public const string Host = "134.209.98.43";
        //public const string Host = "127.0.0.1";
        public const int ServerPort = 12041;

        public static string Sha1Sum2(string str)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(str);
            var sha = new SHA1CryptoServiceProvider();
            return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", string.Empty).ToLower();
        }
    }
}
