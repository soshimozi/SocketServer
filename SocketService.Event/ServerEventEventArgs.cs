using System;

namespace SocketServer.Event
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
