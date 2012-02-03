using System;

namespace SocketServer.Event
{
    public class ConnectionEventArgs : EventArgs
    {
        public bool IsSuccessful
        {
            get;
            set;
        }
    }
}
