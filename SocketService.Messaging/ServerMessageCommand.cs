using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandQueue;

namespace SocketService.Messaging
{
    public class ServerMessageCommand : ICommand
    {
        private IServerMessage _message;

        public ServerMessageCommand(IServerMessage message)
        {
            _message = message;
        }

        public void Execute()
        {
            // execution is simple, just call process message
            _message.ProcessMessage();
        }
    }
}
