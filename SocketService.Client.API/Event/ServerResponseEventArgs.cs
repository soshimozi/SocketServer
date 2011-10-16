using System;
using SocketService.Framework.Client.Response;

namespace SocketService.Client.API.Event
{
    public class ServerResponseEventArgs : EventArgs
    {
        public IResponse Response
        {
            get;
            set;
        }
    }
}
