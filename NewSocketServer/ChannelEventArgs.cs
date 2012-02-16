using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketServer
{
    public class ChannelEventArgs : EventArgs
    {
        #region Constructors
        public ChannelEventArgs(ClientConnection clientConnection)
            : this(clientConnection, null)
        {
        }

        public ChannelEventArgs(ClientConnection clientConnection, Exception ex)
        {
            ClientConnection = clientConnection;
            Exception = ex;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the channel associated with the raising of this event.
        /// </summary>
        public ClientConnection ClientConnection { get; private set; }

        /// <summary>
        /// Gets the exception associated with the raising of this event. 
        /// Will be null if was not raised as the result of an exception.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Gets a value indicating whether there was an exception associated w/ the creation of the event.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has an Exception; otherwise, <c>false</c>.
        /// </value>
        public bool HasException { get { return Exception != null; } }
        #endregion

    }
}
