using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
