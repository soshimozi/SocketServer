using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Core
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageReceivedEventArgs()
        {
        }

        public MessageReceivedEventArgs(string message)
        {
            Message = message;
        }

        public string Message
        {
            get;
            set;
        }
    }
}
