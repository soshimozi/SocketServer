using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using SocketService.Crypto;

namespace CertificateAuthorityServiceLibrary
{
    [DataContract]
    public class CertificateAuthorityHeader
    {
        [DataMember]
        private readonly int _bits;

        private readonly StrongNumberProvider _strongRNG = new StrongNumberProvider();
        public CertificateAuthorityHeader(int bits, 
            BigInteger g, 
            BigInteger prime)
        {
            _bits = bits;
            G = g.ToByteArray().Reverse().ToArray();
            Prime = prime.ToByteArray().Reverse().ToArray();
        }

        [DataMember]
        public byte [] Prime;

        [DataMember]
        public byte [] G;
    }

    [DataContract]
    public class DiffieHellmanProvider
    {
        [DataMember]
        private readonly BigInteger _prime;
        [DataMember]
        private readonly BigInteger _g;
        [DataMember]
        private readonly BigInteger _secret;

        private DiffieHellmanProvider()
        {
            throw new NotImplementedException();
        }

        public DiffieHellmanProvider(
            BigInteger prime,
            BigInteger g,
            BigInteger secret)
        {
            _prime = prime;
            _g = g;
            _secret = secret;
        }

        public BigInteger PublicKey
        {
            get
            {
                return _g.ModPow(_secret, _prime);
            }
        }

        public BigInteger CreatePrivateKey(BigInteger publicKey)
        {
            BigInteger yother = new BigInteger(publicKey.ToString(), 10);
            return yother.ModPow(_secret, _prime);
        }
    }
}
