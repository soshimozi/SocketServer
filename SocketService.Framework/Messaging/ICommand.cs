using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Framework.Messaging
{
    public interface ICommand
    {
        void Execute();
    }
}
