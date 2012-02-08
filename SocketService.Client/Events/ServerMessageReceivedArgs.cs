using System;

namespace SocketServer.Client
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
