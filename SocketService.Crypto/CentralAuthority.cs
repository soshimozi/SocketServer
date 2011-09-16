using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Crypto
{
    [Serializable]
    public class CentralAuthority
    {
        // higher numbers result in more accuracy at the expense of processing time
        private const int Confidence = 30;

        private readonly BigInteger _prime;
        private readonly BigInteger _g;
        private readonly int _bits = 64;
        private readonly StrongNumberProvider _rand = new StrongNumberProvider();

        /// <summary>
        /// Initializes a new instance of the <see cref="CentralAuthority"/> class.
        /// </summary>
        /// <param name="protocol">The protocol.</param>
        public CentralAuthority(CAKeyProtocol protocol)
        {
            _bits = (int)protocol;
            _prime = BigInteger.GenPseudoPrime(_bits, Confidence, _rand);
            _g = (BigInteger)7;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CentralAuthority"/> class.
        /// </summary>
        public CentralAuthority()
        {
            _prime = BigInteger.GenPseudoPrime(_bits, Confidence, _rand);
            _g = (BigInteger)7;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CentralAuthority"/> class.
        /// </summary>
        /// <param name="prime">The prime.</param>
        /// <param name="g">The g.</param>
        public CentralAuthority(BigInteger prime, BigInteger g)
        {
            _prime = prime;
            _g = g;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CentralAuthority"/> class.
        /// </summary>
        /// <param name="prime">The prime.</param>
        /// <param name="g">The g.</param>
        public CentralAuthority(byte [] prime, byte [] g)
        {
            _prime = new BigInteger(prime);
            _g = new BigInteger(g);
        }

        /// <summary>
        /// Gets the provider.
        /// </summary>
        /// <returns></returns>
        public DiffieHellmanProvider GetProvider()
        {
            return new DiffieHellmanProvider(
                _prime,
                _g,
                BigInteger.GenPseudoPrime(_bits, Confidence, _rand));
        }

    }

    public enum CAKeyProtocol
    {
        DH64 = 64,
        DH128 = 128,
        DH256 = 256,
        DH512 = 512,
        DH768 = 768,
        DH1024 = 1024
    }
}
