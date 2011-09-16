using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace SocketService.Crypto
{
    public class DHPublicKey : DHKeyBase
    {
        public DHPublicKey() : base()
        {
        }

        public byte[] PublicKey
        {
            get;
            set;
        }
    }
}
