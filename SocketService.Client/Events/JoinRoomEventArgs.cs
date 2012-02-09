using System;
using SocketServer.Shared.Event;

namespace SocketServer.Client
{
    public class JoinRoomEventArgs : EventArgs
    {
        public JoinRoomEvent Event
        {
            get;
            set;
        }
    }
}
