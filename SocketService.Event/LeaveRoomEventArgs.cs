using System;

namespace SocketService.Event
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
