using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Messaging
{
    public interface ICommand
    {
        void Execute();
    }
}
