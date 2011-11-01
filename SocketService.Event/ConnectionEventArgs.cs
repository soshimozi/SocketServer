using System;

namespace SocketService.Event
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
