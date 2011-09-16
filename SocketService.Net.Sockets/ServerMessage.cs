using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Net.Sockets
{
    [Serializable]
    public class ServerMessage
    {
        private readonly string _message;
        public ServerMessage(string message)
        {
            _message = message;
        }

        public ServerMessage(string format, params object[] args)
        {
            _message = string.Format(format, args);
        }

        public string Message
        {
            get
            {
                return _message;
            }
        }
    }
}
