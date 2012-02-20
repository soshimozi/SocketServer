using System;

namespace SocketServer.Client
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
