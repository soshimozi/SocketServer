using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandQueue;
using SocketService.ServerMessage;

namespace SocketService.Command
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
            // pass in the server context, ah, that's the trick isn't it?
            _message.ProcessMessage();
        }
    }
}
