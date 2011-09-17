using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Framework.Response
{
    [Serializable]
    public class NegotiateKeysResponse
    {
        public byte[] RemotePublicKey
        {
            get;
            set;
        }

    }
}
