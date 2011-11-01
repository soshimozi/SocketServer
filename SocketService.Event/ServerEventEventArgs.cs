using System;

namespace SocketService.Event
{
    public class ServerEventEventArgs : EventArgs
    {
        public IEvent ServerEvent
        {
            get;
            set;
        }

    }
}
