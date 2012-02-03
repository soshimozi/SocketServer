using System;
using SocketServer.Shared.Response;

namespace SocketServer.Event
{
    public class ServerResponseEventArgs : EventArgs
    {
        public IServerResponse Response
        {
            get;
            set;
        }
    }
}
