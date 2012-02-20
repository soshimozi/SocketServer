using System;

namespace SocketServer.Net.Client
{
    public class Connection
    {
        public Connection(Guid clientId)
        {
            ClientId = clientId;
        }
        /// <summary>
        /// Gets or sets the client id.
        /// </summary>
        /// <value>
        /// The client id.
        /// </value>
        public Guid ClientId { get; set; }

    }
}