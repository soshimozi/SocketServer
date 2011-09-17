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
    public class ChangeRoomRequest
    {
        public string RoomName
        {
            get;
            set;
        }
    }
}
