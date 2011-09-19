using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.SharedObjects;

namespace SocketService.Framework.Client.Response
{
    [Serializable]
    public class GetRoomVariableResponse
    {

        public string Room
        {
            get;
            set;
        }
    
        public string Name
        {
            get;
            set;
        }
    
        /// <summary>
        /// Gets or sets the server object.
        /// </summary>
        /// <value>
        /// The server object.
        /// </value>
        public RoomVariable RoomVariable
        {
            get;
            set;
        }
    }
}
