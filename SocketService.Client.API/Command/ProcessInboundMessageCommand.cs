using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Client.API.Command
{
    class ProcessInboundMessageCommand : ICommand
    {
        private readonly object _message;
        public ProcessInboundMessageCommand(object message)
        {
            _message = message;
        }

        public void Execute()
        {
        }
    }
}
