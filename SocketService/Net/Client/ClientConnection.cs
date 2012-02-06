using System;
using SocketServer.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using SocketServer.Shared.Header;

namespace SocketServer.Net.Client
{
    public class ClientConnection //: Connection
    {
        private readonly ServerAuthority _sa = new ServerAuthority(256, 30);

        public ClientConnection()
        {
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
            get
            {
                lock (this)
                {
                    return _sa;
                }
            }

        }

        private ClientBuffer _buffer = new ClientBuffer();
        public ClientBuffer ClientBuffer
        {
            get
            {
                lock (this)
                {
                    return _buffer;
                }
            }
            set
            {
                lock (this)
                {
                    _buffer = value;
                }
            }
        }

        ProtocolState _state;
        public ProtocolState CurrentState
        {
            get
            {
                lock (this)
                {
                    return _state;
                }
            }

            set
            {
                lock (this)
                {
                    _state = value;
                }
            }
        }

        private RequestHeader _requestHeader = null;
        public RequestHeader RequestHeader
        {
            get
            {
                lock (this)
                {
                    return _requestHeader;
                }
            }

            set
            {
                lock (this)
                {
                    _requestHeader = value;
                }
            }
        }

    }

}