using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.SharedObjects;

namespace SocketService.Response
{
    [Serializable]
    public class ListUsersInRoomResponse
    {
        public ServerUser[] Users
        {
            get;
            set;
        }
    }
}
