using System;
using SocketServer.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;

namespace SocketServer.Net.Client
{
    public class ClientConnection //: Connection
    {
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

        public DHProvider Provider
        { 
            get; set; 
        }
    }

}