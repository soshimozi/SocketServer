using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Client.API.Event
{
    public class LeaveRoomEventArgs : EventArgs
    {
        public int RoomId
        {
            get;
            set;
        }
    }
}
