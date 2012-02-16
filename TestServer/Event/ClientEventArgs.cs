using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestServer.Network;

namespace TestServer.Event
{
    public class ClientEventArgs : EventArgs
    {

        #region Constructors
        public ClientEventArgs(ClientConnection clientConnection)
            : this(clientConnection, null)
        {
        }

        public ClientEventArgs(ClientConnection clientConnection, Exception ex)
        {
            ClientConnection = clientConnection;
            Error = ex;
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
        public Exception Error { get; private set; }

        /// <summary>
        /// Gets a value indicating whether there was an exception associated w/ the creation of the event.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has an Exception; otherwise, <c>false</c>.
        /// </value>
        public bool HasError { get { return Error != null; } }
        #endregion
    }


}
