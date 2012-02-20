using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Math;
using System.Security.Cryptography;

namespace SocketServer.Crypto
{
    public class SharedKeyCrypto : ICrypto
    {
        private const int DefaultKeySize = 128;

        private readonly DHParameters sharedParameters;
        private readonly byte[] encodedPublicKey;
        private readonly AsymmetricCipherKeyPair keyPair;

        public SharedKeyCrypto(DHParameters sharedParameters, AsymmetricCipherKeyPair keyPair, byte[] encodedPublicKey)
        {
            this.encodedPublicKey = encodedPublicKey;
            this.sharedParameters = sharedParameters;
            this.keyPair = keyPair;
        }

        public string Encrypt(string plainText)
        {
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(
                Encrypt(
                    plainBytes,
                    GetRijndaelManaged(GeneratePassword(), DefaultKeySize)
                )
            );
        }

        public string Decrypt(string cipherText)
        {
            var encryptedBytes = Convert.FromBase64String(cipherText);
            return Encoding.UTF8.GetString(
                Decrypt(
                    encryptedBytes,
                    GetRijndaelManaged(GeneratePassword(), DefaultKeySize)
                )
            );

        }

        private string GeneratePassword()
        {
            DHPublicKeyParameters publicKey = new DHPublicKeyParameters(
                ((DHPublicKeyParameters)PublicKeyFactory.CreateKey(encodedPublicKey)).Y, sharedParameters);

            IBasicAgreement agreement = AgreementUtilities.GetBasicAgreement("DH");
            agreement.Init(keyPair.Private);

            BigInteger agreementValue = agreement.CalculateAgreement(publicKey);
            return agreementValue.ToString(16);
        }

        public byte[] Encrypt(byte[] plainBytes)
        {
            return Encrypt(plainBytes, GetRijndaelManaged(GeneratePassword(), DefaultKeySize));
        }

        public byte[] Decrypt(byte[] cipherBytes)
        {
            return Decrypt(cipherBytes, GetRijndaelManaged(GeneratePassword(), DefaultKeySize));
        }

        private static byte[] Encrypt(byte[] plainBytes, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateEncryptor()
                .TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }

        private static byte[] Decrypt(byte[] encryptedData, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateDecryptor()
                .TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        }

        private static RijndaelManaged GetRijndaelManaged(String secretKey, int keySize)
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

        public AsymmetricKeyParameter PublicKey
        {
            get { return keyPair.Public; }
        }

    }

}
