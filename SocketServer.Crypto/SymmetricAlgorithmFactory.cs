using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace SocketServer.Crypto
{
    class SymmetricAlgorithmFactory
    {
        public static RijndaelManaged GetRijndaelManaged(String secretKey, int keySize)
        {
            var keyBytes = new byte[keySize / 8];
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            Array.Copy(secretKeyBytes, keyBytes, Math.Min(keyBytes.Length, secretKeyBytes.Length));
            return new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = keySize,
                BlockSize = keySize,
                Key = keyBytes,
                IV = keyBytes
            };
        }
    }
}
