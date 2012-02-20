using System;

namespace SocketServer.Event
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
