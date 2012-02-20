using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace SocketService.Framework.Client.Crypto
{
    public static class StringExtentions
    {
        private static byte[] salt = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0xF1, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0xF1 };

        /// <summary>
        /// Encrypts the specified secret.
        /// </summary>
        /// <param name="secret">The secret.</param>
        /// <param name="key">The key.</param>
        /// <param name="iv">The iv.</param>
        /// <returns></returns>
        public static byte[] Encrypt(this string secret, byte[] key, out byte[] iv)
        {
            using( TripleDES des = new TripleDESCryptoServiceProvider())
            {
                Rfc2898DeriveBytes db = new Rfc2898DeriveBytes(key, des.IV, 50);
                des.Key = db.GetBytes(des.KeySize / 8);
                iv = des.IV;

                // Encrypt the message
                byte[] plaintextMessage = Encoding.UTF8.GetBytes(secret);

                using (MemoryStream ciphertext = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ciphertext, des.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(plaintextMessage, 0, plaintextMessage.Length);
                    }

                    return ciphertext.ToArray();
                }
            }
        }

        /// <summary>
        /// Decrypts the specified cipher.
        /// </summary>
        /// <param name="cipher">The cipher.</param>
        /// <param name="key">The key.</param>
        /// <param name="iv">The iv.</param>
        /// <returns></returns>
        public static string Decrypt(this byte[] cipher, byte[] key, byte[] iv)
        {
            using (TripleDES aes = new TripleDESCryptoServiceProvider())
            {
                Rfc2898DeriveBytes db = new Rfc2898DeriveBytes(key, iv, 50);
                aes.Key = db.GetBytes(aes.KeySize / 8);
                aes.IV = iv;

                // Encrypt the message
                using (MemoryStream plainText = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(plainText, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipher, 0, cipher.Length);
                    }
                    
                    return Encoding.UTF8.GetString(plainText.ToArray());
                }
            }
        }
    }
}
