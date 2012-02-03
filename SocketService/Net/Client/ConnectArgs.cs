using System;
using System.Net.Sockets;

namespace SocketServer.Net.Client
{
    public class ConnectArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectArgs"/> class.
        /// </summary>
        public ConnectArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectArgs"/> class.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="rawSocket">The raw socket.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public ConnectArgs(Guid clientId, Socket rawSocket, string remoteAddress)
        {
            ClientId = clientId;
            RemoteAddress = remoteAddress;
            RawSocket = rawSocket;
        }

        /// <summary>
        /// Gets or sets the client id.
        /// </summary>
        /// <value>
        /// The client id.
        /// </value>
        public Guid ClientId { get; set; }

        /// <summary>
        /// Gets or sets the raw socket.
        /// </summary>
        /// <value>
        /// The raw socket.
        /// </value>
        public Socket RawSocket { get; set; }

        /// <summary>
        /// Gets or sets the remote address.
        /// </summary>
        /// <value>
        /// The remote address.
        /// </value>
        public string RemoteAddress { get; set; }
    }
}