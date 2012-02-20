using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketServer.Shared.Network;
using SocketServer.Messages;

namespace SocketServer.Handler
{
    public class MessageOneHandler : IRequestHandler<MessageOne>
    {
        public void HandleRequest(MessageOne request, ClientConnection connection)
        {
        }
    }
}
