using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SADCrypt.Helper
{
    public class AesSymKey
    {
        private readonly SymmetricAlgorithm _client;
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public AesSymKey(SymmetricAlgorithm client, byte[] key, byte[] iv)
        {
            this._client = client;
            this._key = key;
            this._iv = iv;
        }

        /// <summary>
        /// This method return the encryptor/decryptor object depending on the boolean parameter
        /// which then is used to decrypt or encrypt text based on the key and iv.
        /// </summary>
        /// <param name="encrypt">true to get encryptor, false to get decryptor</param>
        /// <returns></returns>
        public ICryptoTransform GetKeyIV(bool encrypt)
        {
            if (encrypt)
                return _client.CreateEncryptor(_key, _iv);
            else
                return _client.CreateDecryptor(_key, _iv);
        }

        public byte[] GetKey()
        {
            return _key;
        }

        public byte[] GetIV()
        {
            return _iv;
        }
    }
}
