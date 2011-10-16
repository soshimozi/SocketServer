using System;

namespace SocketService.Client.API.Event
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
