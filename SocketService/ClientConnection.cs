using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.SocketLib;
using SocketService.Crypto;

namespace SocketService
{
    public class ClientConnection
    {
        public ClientConnection(DataSocket client, CentralAuthority.DiffieHellmanProvider provider)
        {
            DataSocket = client;
            Provider = provider;
        }

        public DiffieHellmanKey RemotePublicKey
        {
            get;
            set;
        }

        public CentralAuthority.DiffieHellmanProvider Provider
        {
            get;
            private set;
        }

        public DataSocket DataSocket
        {
            get;
            private set;
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
