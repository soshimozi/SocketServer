using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Framework.Request
{
    [Serializable]
    public class ListUsersInRoomRequest
    {
        public string RoomName
        {
            get;
            set;
        }
    }
}
