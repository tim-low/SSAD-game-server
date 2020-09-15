using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using SADCrypt.Helper;

namespace SADCrypt
{
    public class Supercrypt
    {
        private AesSymKey _ca;

        public Supercrypt()
        {
            SymmetricAlgorithm sa = new AesManaged();
            _ca = new AesSymKey(sa, sa.Key, sa.IV);
        }

        public Supercrypt(byte[] key, byte[] iv)
        {
            SymmetricAlgorithm x = new AesManaged();
            _ca = new AesSymKey(x, key, iv);
        }

        public byte[] Encrypt(byte[] data)
        {
            return Encryption.Encrypt(_ca.GetKeyIV(true), data);
        }

        public byte[] Decrypt(byte[] encryptedData)
        {
            return Decryption.Decrypt(_ca.GetKeyIV(false), encryptedData);
        }

        public byte[] GetKey()
        {
            return _ca.GetKey();
        }

        public byte[] GetIV()
        {
            return _ca.GetIV();
        }
    }
}
