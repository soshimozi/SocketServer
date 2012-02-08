﻿using System;
using SocketServer.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using SocketServer.Shared.Header;
using SocketServer.Shared;

namespace SocketServer.Net.Client
{
    public class ClientConnection //: Connection
    {
        private ServerAuthority _sa = null;

        public ClientConnection(ClientBuffer buffer)
        {
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
            get { return _sa ?? (_sa = ServerAuthorityFactory.CreateServerAuthority());  }
            //set;
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