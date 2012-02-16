using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketServer
{
    public class ConnectedMessage : Message
    {
        private string serverName = string.Empty;

        public ConnectedMessage()
        {
        }

        public ConnectedMessage(string serverName)
        {
            ServerName = serverName;
        }

        /// <summary>
        /// Gets or sets the name of the server that sent this message.  This is helpful for
        /// client connections to know which server they have connected to.
        /// </summary>
        public string ServerName
        {
            get { return serverName; }
            set { serverName = value; }
        }

        /// <summary>
        /// Indexer to add arbitrary headers to this message.  This is useful to provide
        /// connection information to incoming client connections.
        /// </summary>
        /// <param name="header">The key string of the header to get or retrieve</param>
        /// <returns>The header string if it exists, or null otherwise.</returns>
        public string this[string header]
        {
            get
            {
                if (!this.HeaderExists(header)) return null;
                return this.GetHeaderValue(header);
            }
            set { this.AddHeader(header, value); }
        }

        public override void MapFromHeaders()
        {
            if (HeaderExists("SERVERNAME"))
                serverName = GetHeaderValue("SERVERNAME");
        }

        public override void MapToHeaders()
        {
            if (serverName != string.Empty)
                AddHeader("SERVERNAME", serverName);
        }
    }

}
