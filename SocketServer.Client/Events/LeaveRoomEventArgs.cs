using System;

namespace SocketServer.Client
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
