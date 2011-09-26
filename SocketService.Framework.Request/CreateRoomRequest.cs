using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Framework.Request
{
    [Serializable]
    /// <summary>
    /// Creates a room if it doesn't exist and switches requestor to that room
    /// </summary>
    public class CreateRoomRequest
    {
        public string ZoneName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the room.
        /// </summary>
        /// <value>
        /// The name of the room.
        /// </value>
        public string RoomName
        {
            get;
            set;
        }
    }
}
