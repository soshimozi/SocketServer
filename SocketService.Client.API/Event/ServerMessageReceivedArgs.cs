using System;

namespace SocketService.Client.API.Event
{
    public class ServerMessageReceivedArgs : EventArgs
    {
        public string Message
        {
            get;
            set;
        }
    
    }
}
