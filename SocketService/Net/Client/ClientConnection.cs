using System;
using SocketServer.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using SocketServer.Shared.Header;

namespace SocketServer.Net.Client
{
    public class ClientConnection //: Connection
    {
        //private readonly ServerAuthority _sa = new ServerAuthority(256, 30);

        public ClientConnection(ServerAuthority sa, ClientBuffer buffer)
        {
            ServerAuthority = sa;
            ClientBuffer = buffer;
        }

        /// <summary>
        /// Gets or sets the client id.
        /// </summary>
        /// <value>
        /// The client id.
        /// </value>
        public Guid ClientId { get; set; }

        /// <summary>
        /// Gets or sets the remote public key.
        /// </summary>
        /// <value>
        /// The remote public key.
        /// </value>
        //public AsymmetricKeyParameter RemotePublicKey { get; set; }

        //public DHParameters Parameters { get; set; }

        public ServerAuthority ServerAuthority
        {
            get;
            set;
        }

        public ClientBuffer ClientBuffer
        {
            get; set;
        }

        public ProtocolState CurrentState
        {
            get; set;
        }

        public RequestHeader RequestHeader
        {
            get; set;
        }
    }

}