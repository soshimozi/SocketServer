using System;
using SocketService.Crypto;

namespace SocketService.Net.Client
{
    public class ClientConnection //: Connection
    {

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
        public DiffieHellmanKey RemotePublicKey { get; set; }

        /// <summary>
        /// Gets or sets the provider.
        /// </summary>
        /// <value>
        /// The provider.
        /// </value>
        public DiffieHellmanProvider SecureKeyProvider { get; set; }

        /// <summary>
        /// Gets or sets the state of the connection.
        /// </summary>
        /// <value>
        /// The state of the connection.
        /// </value>
        public ConnectionState ConnectionState { get; set; }
    }

    public enum ConnectionState
    {
        NegotiateKeyPair,
        Connected
    }
}