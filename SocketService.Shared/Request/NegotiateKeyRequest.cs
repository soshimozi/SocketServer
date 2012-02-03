using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketServer.Shared.Request
{
    [Serializable]
    public class NegotiateKeyRequest
    {
        public byte[] RemotePublicKey;
    }
}
