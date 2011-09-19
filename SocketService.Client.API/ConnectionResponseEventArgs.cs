using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Client.API
{
    public class ConnectionResponseEventArgs : EventArgs
    {
        public bool IsSuccessful
        {
            get;
            set;
        }
    }
}
