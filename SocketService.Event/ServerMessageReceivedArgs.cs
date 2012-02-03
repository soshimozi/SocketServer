using System;

namespace SocketServer.Event
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
