using System;
using SocketServer.Shared.Response;

namespace SocketServer.Client
{
    public class ServerResponseEventArgs : EventArgs
    {
        public object Response
        {
            get;
            set;
        }
    }
}
