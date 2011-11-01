using System;

namespace SocketService.Event
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
