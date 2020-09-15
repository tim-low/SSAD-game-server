using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace SADCrypt
{
    public class Decryption
    {
        internal static byte[] Decrypt(ICryptoTransform decryptor, byte[] encryptedData)
        {

            using (MemoryStream ms = new MemoryStream(encryptedData))
            {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                {
                    cs.Write(encryptedData, 0, encryptedData.Length);
                }
                return ms.ToArray();
            }
        }
    }
}
