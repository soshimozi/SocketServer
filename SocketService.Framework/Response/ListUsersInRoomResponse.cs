using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.SharedObjects;

namespace SocketService.Framework.Response
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
