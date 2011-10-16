using System;

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
