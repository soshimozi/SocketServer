using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Request
{
    [Serializable]
    public class NegotiateKeysRequest
    {
        public NegotiateKeysRequest()
        {
        }
    
        public NegotiateKeysRequest(byte[] key)
        {
            PublicKey = key;
        }

        public byte[] PublicKey
        {
            get;
            set;
        }

    }
}
