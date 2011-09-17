using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.SharedObjects;

namespace SocketService.Framework.Response
{
    [Serializable]
    public class GetRoomVariableResponse
    {
        /// <summary>
        /// Gets or sets the server object.
        /// </summary>
        /// <value>
        /// The server object.
        /// </value>
        public ServerObject ServerObject
        {
            get;
            set;
        }
    }
}
