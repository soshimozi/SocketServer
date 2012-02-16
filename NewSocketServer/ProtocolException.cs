using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketServer
{
    public class ProtocolException : Exception
    {
        public ProtocolException(string message)
            : base(message)
        {
        }
    }
}
