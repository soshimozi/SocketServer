using System;
using SocketServer.Client;

namespace SocketServer.Client
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
