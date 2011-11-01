using System;
using SocketService.Core.Crypto;

namespace SocketService.Net.Client
{
    public class ClientConnection : Connection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientConnection"/> class.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        public ClientConnection(Guid clientId) : base(clientId)
        {
        }

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
        public DiffieHellmanProvider Provider { get; set; }

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