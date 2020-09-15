using System;
using System.IO;
using System.Security.Cryptography;

namespace SADCrypt
{
    /// <summary>
    /// Encryption class to encrypt data using symmetric key encryption
    /// </summary>
    public class Encryption
    {
        internal static byte[] Encrypt(ICryptoTransform encryptor, byte[] data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(data);
                }
                return ms.ToArray();
            }
        }
    }
}
