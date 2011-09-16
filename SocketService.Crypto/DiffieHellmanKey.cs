using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Crypto
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class DiffieHellmanKey
    {
        private readonly BigInteger _value;
        private readonly BigInteger _g;
        private readonly BigInteger _prime;

        private DiffieHellmanKey()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiffieHellmanKey"/> class.
        /// </summary>
        /// <param name="prime">The prime.</param>
        /// <param name="g">The g.</param>
        /// <param name="y">The y.</param>
        public DiffieHellmanKey(BigInteger prime, BigInteger g, BigInteger y)
        {
            _g = g;
            _prime = prime;
            _value = y;
        }

        /// <summary>
        /// Gets the G.
        /// </summary>
        public BigInteger G
        {
            get { return _g; }
        }

        /// <summary>
        /// Gets the prime.
        /// </summary>
        public BigInteger Prime
        {
            get { return _prime; }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public BigInteger Value
        {
            get
            {
                return _value;
            }
        }

        /// <summary>
        /// Convert to byte array.
        /// </summary>
        /// <returns></returns>
        public byte[] ToByteArray()
        {
            return _value.ToByteArray();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return _value.ToString(10);
        }
    }
}
