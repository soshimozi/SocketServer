using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using SocketService.Crypto;

namespace CertificateAuthorityServiceLibrary
{
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.Single)]
    public class CertificateAuthorityService : ICertificateAuthorityService
    {
        private readonly StrongNumberProvider _rand = new StrongNumberProvider();
        private readonly BigInteger _g = (BigInteger)7;
        private readonly BigInteger _prime;
        private readonly int _bitSize = 1024;

        public CertificateAuthorityService()
        {
            _prime = BigInteger.GenPseudoPrime(_bitSize, 30, _rand);
        }
    
        public CertificateAuthorityHeader GetCertificateAuthority()
        {
            return new CertificateAuthorityHeader(_bitSize, _g, _prime);
        }
    }
}
