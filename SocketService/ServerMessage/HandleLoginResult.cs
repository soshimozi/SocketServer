using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.ServerMessage
{
    public class HandleLoginResult : IServerMessage
    {
        private Guid _clientId;
        private bool _success;

        public HandleLoginResult(Guid clientId, bool success)
        {
            _clientId = clientId;
            _success = success;
        }

        public void ProcessMessage()
        {
            // send result of login back to client
        }
    }
}
