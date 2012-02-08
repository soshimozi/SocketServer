using System;
using SocketServer.Shared.Response;

namespace SocketServer.Client
{
    public class ServerResponseEventArgs : EventArgs
    {
        public IEvent Response
        {
            get;
            set;
        }
    }
}
