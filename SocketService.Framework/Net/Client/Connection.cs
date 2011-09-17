using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.Crypto;

namespace SocketService.Framework.Net.Client
{
    public class Connection
    {
        public Connection(Guid clientId)
        {
            ClientId = clientId;
        }

        public Guid ClientId
        {
            get;
            set;
        }

        public DiffieHellmanKey RemotePublicKey
        {
            get;
            set;
        }

        public DiffieHellmanProvider Provider
        {
            get;
            set;
        }

        public ConnectionState ConnectionState
        {
            get;
            set;
        }
    }

    public enum ConnectionState
    {
        NegotiateKeyPair,
        Connected
    }
}
