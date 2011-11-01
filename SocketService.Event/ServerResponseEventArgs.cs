using System;
using SocketService.Shared.Response;

namespace SocketService.Event
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
