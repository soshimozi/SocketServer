using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.Client.SharedObjects;

namespace SocketService.Framework.Client.Response
{
    [Serializable]
    public class ListUsersInRoomResponse
    {
        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        /// <value>
        /// The users.
        /// </value>
        public ServerUser[] Users
        {
            get;
            set;
        }
    }
}
