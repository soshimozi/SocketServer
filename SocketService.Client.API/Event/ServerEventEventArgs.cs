using System;
using SocketService.Framework.Client.Event;

namespace SocketService.Client.API.Event
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
