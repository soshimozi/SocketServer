using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace SocketServer.Crypto
{
    public class RijndaelCrypto
    {
        private const int DefaultKeySize = 128;

        public string Encrypt(string plainText, string password)
        {
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(
                Encrypt(
                    plainBytes,
                    SymmetricAlgorithmFactory.GetRijndaelManaged(password, DefaultKeySize)
                )
            );
        }

        public string Decrypt(string cipherText, string password)
        {
            var encryptedBytes = Convert.FromBase64String(cipherText);
            return Encoding.UTF8.GetString(
                Decrypt(
                    encryptedBytes, 
                    SymmetricAlgorithmFactory.GetRijndaelManaged(password, DefaultKeySize)
                )
            );

        }

        public static byte[] Encrypt(byte[] plainBytes, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateEncryptor()
                .TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }

        public static byte[] Decrypt(byte[] encryptedData, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateDecryptor()
                .TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        }
    }
}
