using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace SocketService.Framework.Net.Client
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
		public DisconnectedArgs(Guid clientId)
		{
			ClientId = clientId;
		}

        /// <summary>
        /// Gets or sets the client id.
        /// </summary>
        /// <value>
        /// The client id.
        /// </value>
        public Guid ClientId
        {
            get;
            set;
        }

    }
}
