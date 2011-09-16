using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace SocketService.Crypto
{
    public class Wrapper : IDisposable
    {
        private readonly SymmetricAlgorithm _algorithm;

        private Wrapper()
        {
            throw new NotImplementedException();
        }

        private Wrapper(SymmetricAlgorithm algorithm, ICryptoTransform transform)
        {
            _algorithm = algorithm;
            _transformer = transform;
        }

        ~Wrapper()
        {
            Dispose(false);
        }

        public byte[] IV
        {
            get { return _algorithm.IV; }
        }

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

            Rfc2898DeriveBytes db = new Rfc2898DeriveBytes(key, algorithm.IV, entropy);
            algorithm.Key = db.GetBytes(algorithm.KeySize / 8);

            return new Wrapper(algorithm, algorithm.CreateEncryptor());
        }

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

            Rfc2898DeriveBytes db = new Rfc2898DeriveBytes(key, iv, entropy);
            algorithm.Key = db.GetBytes(algorithm.KeySize / 8);
            algorithm.IV = iv;

            return new Wrapper(algorithm, algorithm.CreateDecryptor());
        }

        ICryptoTransform _transformer;
        public ICryptoTransform CryptoTransformer
        {
            get
            {
                return _transformer;
            }
        
        }

        public byte[] EncryptString(string secret)
        {
            using (MemoryStream cipherText = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(cipherText, _transformer, CryptoStreamMode.Write))
                {
                     byte[] plainText = Encoding.UTF8.GetBytes(secret);
                     cs.Write(plainText, 0, plainText.Length);
                }

                return cipherText.ToArray();
            }
        }

        public byte[] Encrypt(byte[] sensitive)
        {
            using (MemoryStream cipherStream = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(cipherStream, _transformer, CryptoStreamMode.Write))
                {
                    cs.Write(sensitive, 0, sensitive.Length);
                }

                return cipherStream.ToArray();
            }
        }

        public byte[] Decrypt(byte[] cipher)
        {
            // Decrypt the message
            using (MemoryStream decrypted = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(decrypted, _transformer, CryptoStreamMode.Write))
                {
                    cs.Write(cipher, 0, cipher.Length);
                }

                return decrypted.ToArray();
            }

        }

        public string DecryptString(string cipherString)
        {
            byte[] cipher = System.Text.Encoding.UTF8.GetBytes(cipherString);

            // Decrypt the message
            using (MemoryStream plainText = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(plainText, _transformer, CryptoStreamMode.Write))
                {
                    cs.Write(cipher, 0, cipher.Length);
                }

                return Encoding.UTF8.GetString(plainText.ToArray());
            }
        }

        public string DecryptString(byte [] cipher)
        {
            // Decrypt the message
            using (MemoryStream plainText = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(plainText, _transformer, CryptoStreamMode.Write))
                {
                    cs.Write(cipher, 0, cipher.Length);
                }

                return Encoding.UTF8.GetString(plainText.ToArray());
            }
        }

        private bool disposed = false;

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
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    _algorithm.Dispose();
                }

                // Note disposing has been done.
                disposed = true;

            }
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
