using System;

namespace SocketServer.Shared.Network
{
    public class DisconnectedArgs : EventArgs
    {
        public DisconnectedArgs()
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="DisconnectedArgs"/> class.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        public DisconnectedArgs(ClientConnection connection)
        {
            Connection = connection;
        }

        /// <summary>
        /// Gets or sets the client id.
        /// </summary>
        /// <value>
        /// The client id.
        /// </value>
        public ClientConnection Connection { get; set; }
    }
}