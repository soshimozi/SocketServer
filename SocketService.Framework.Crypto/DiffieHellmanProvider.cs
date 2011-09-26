using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Crypto
{
    /// <summary>
    /// 
    /// </summary>
    public class DiffieHellmanProvider
    {
        private readonly BigInteger _prime;
        private readonly BigInteger _g;
        private readonly BigInteger _secret;

        protected DiffieHellmanProvider()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiffieHellmanProvider"/> class.
        /// </summary>
        /// <param name="prime">The prime.</param>
        /// <param name="g">The g.</param>
        /// <param name="secret">The secret.</param>
        public DiffieHellmanProvider(
            BigInteger prime,
            BigInteger g,
            BigInteger secret)
        {
            _prime = prime;
            _g = g;
            _secret = secret;
        }

        /// <summary>
        /// Gets the public key.
        /// </summary>
        public DiffieHellmanKey PublicKey
        {
            get
            {
                return new DiffieHellmanKey(_prime, _g, _g.ModPow(_secret, _prime));
            }
        }

        /// <summary>
        /// Creates the private key.
        /// </summary>
        /// <param name="publicKey">The public key.</param>
        /// <returns></returns>
        public DiffieHellmanKey CreatePrivateKey(DiffieHellmanKey publicKey)
        {
            BigInteger yother = new BigInteger(publicKey.ToByteArray());
            return new DiffieHellmanKey(_prime, _g, yother.ModPow(_secret, _prime));
        }

        /// <summary>
        /// Creates the private key.
        /// </summary>
        /// <param name="publicKey">The public key.</param>
        /// <returns></returns>
        public DiffieHellmanKey CreatePrivateKey(byte[] publicKey)
        {
            BigInteger yother = new BigInteger(publicKey);
            return new DiffieHellmanKey(_prime, _g, yother.ModPow(_secret, _prime));
        }

        /// <summary>
        /// Imports the specified key data.
        /// </summary>
        /// <param name="keyData">The key data.</param>
        /// <returns></returns>
        public DiffieHellmanKey Import(byte[] keyData)
        {
            return new DiffieHellmanKey(_prime, _g, new BigInteger(keyData));
        }
    }

}
