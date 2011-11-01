using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace SocketService.Core.Crypto
{
    public class Wrapper : IDisposable
    {
        private readonly SymmetricAlgorithm _algorithm;
        private readonly ICryptoTransform _transformer;

        private Wrapper(SymmetricAlgorithm algorithm, ICryptoTransform transform)
        {
            _algorithm = algorithm;
            _transformer = transform;
        }

        ~Wrapper()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the IV.
        /// </summary>
        public byte[] IV
        {
            get { return _algorithm.IV; }
        }

        /// <summary>
        /// Creates the encryptor.
        /// </summary>
        /// <param name="algType">Type of the alg.</param>
        /// <param name="key">The key.</param>
        /// <param name="entropy">The entropy.</param>
        /// <returns></returns>
        public static Wrapper CreateEncryptor(AlgorithmType algType, byte [] key, int entropy = 50)
        {
            SymmetricAlgorithm algorithm = null;
            switch (algType)
            {
                case AlgorithmType.AES:
                    algorithm = new AesCryptoServiceProvider();
                    break;

                case AlgorithmType.DES:
                    algorithm = new DESCryptoServiceProvider();
                    break;

                case AlgorithmType.TripleDES:
                    algorithm = new TripleDESCryptoServiceProvider();
                    break;
            }

            if (algorithm != null)
            {
                var db = new Rfc2898DeriveBytes(key, algorithm.IV, entropy);
                algorithm.Key = db.GetBytes(algorithm.KeySize / 8);
            }

            if (algorithm != null) return new Wrapper(algorithm, algorithm.CreateEncryptor());

            return null;
        }

        /// <summary>
        /// Creates the decryptor.
        /// </summary>
        /// <param name="algType">Type of the alg.</param>
        /// <param name="key">The key.</param>
        /// <param name="iv">The iv.</param>
        /// <param name="entropy">The entropy.</param>
        /// <returns></returns>
        public static Wrapper CreateDecryptor(AlgorithmType algType, byte[] key, byte[] iv, int entropy = 50)
        {
            SymmetricAlgorithm algorithm = null;
            switch (algType)
            {
                case AlgorithmType.AES:
                    algorithm = new AesCryptoServiceProvider();
                    break;

                case AlgorithmType.DES:
                    algorithm = new DESCryptoServiceProvider();
                    break;

                case AlgorithmType.TripleDES:
                    algorithm = new TripleDESCryptoServiceProvider();
                    break;
            }

            var db = new Rfc2898DeriveBytes(key, iv, entropy);
            if (algorithm != null)
            {
                algorithm.Key = db.GetBytes(algorithm.KeySize / 8);
                algorithm.IV = iv;

                return new Wrapper(algorithm, algorithm.CreateDecryptor());
            }

            return null;
        }

        /// <summary>
        /// Gets the crypto transformer.
        /// </summary>
        public ICryptoTransform CryptoTransformer
        {
            get
            {
                return _transformer;
            }
        
        }

        /// <summary>
        /// Encrypts the string.
        /// </summary>
        /// <param name="secret">The secret.</param>
        /// <returns></returns>
        public byte[] EncryptString(string secret)
        {
            using (var cipherText = new MemoryStream())
            {
                using (var cs = new CryptoStream(cipherText, _transformer, CryptoStreamMode.Write))
                {
                     byte[] plainText = Encoding.UTF8.GetBytes(secret);
                     cs.Write(plainText, 0, plainText.Length);
                }

                return cipherText.ToArray();
            }
        }

        /// <summary>
        /// Encrypts the specified sensitive.
        /// </summary>
        /// <param name="sensitive">The sensitive.</param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] sensitive)
        {
            using (var cipherStream = new MemoryStream())
            {
                using (var cs = new CryptoStream(cipherStream, _transformer, CryptoStreamMode.Write))
                {
                    cs.Write(sensitive, 0, sensitive.Length);
                }

                return cipherStream.ToArray();
            }
        }

        /// <summary>
        /// Decrypts the specified cipher.
        /// </summary>
        /// <param name="cipher">The cipher.</param>
        /// <returns></returns>
        public byte[] Decrypt(byte[] cipher)
        {
            // Decrypt the message
            using (var decrypted = new MemoryStream())
            {
                using (var cs = new CryptoStream(decrypted, _transformer, CryptoStreamMode.Write))
                {
                    cs.Write(cipher, 0, cipher.Length);
                }

                return decrypted.ToArray();
            }

        }

        /// <summary>
        /// Decrypts the string.
        /// </summary>
        /// <param name="cipherString">The cipher string.</param>
        /// <returns></returns>
        public string DecryptString(string cipherString)
        {
            var cipher = Encoding.UTF8.GetBytes(cipherString);

            // Decrypt the message
            using (var plainText = new MemoryStream())
            {
                using (var cs = new CryptoStream(plainText, _transformer, CryptoStreamMode.Write))
                {
                    cs.Write(cipher, 0, cipher.Length);
                }

                return Encoding.UTF8.GetString(plainText.ToArray());
            }
        }

        /// <summary>
        /// Decrypts the string.
        /// </summary>
        /// <param name="cipher">The cipher.</param>
        /// <returns></returns>
        public string DecryptString(byte [] cipher)
        {
            // Decrypt the message
            using (var plainText = new MemoryStream())
            {
                using (var cs = new CryptoStream(plainText, _transformer, CryptoStreamMode.Write))
                {
                    cs.Write(cipher, 0, cipher.Length);
                }

                return Encoding.UTF8.GetString(plainText.ToArray());
            }
        }

        private bool _disposed;

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (_disposed) return;

            // If disposing equals true, dispose all managed
            // and unmanaged resources.
            if (disposing)
            {
                // Dispose managed resources.
                _algorithm.Dispose();
            }

            // Note disposing has been done.
            _disposed = true;
        }

    }

    public enum AlgorithmType
    {
        AES,
        DES,
        TripleDES,
        Rjindal,
        None
    }
}
