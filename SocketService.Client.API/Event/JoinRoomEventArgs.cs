using System;
using SocketService.Framework.Client.Event;

namespace SocketService.Client.API.Event
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
