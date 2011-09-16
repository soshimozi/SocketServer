using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Commands
{
    public interface ICommand
    {
        void Execute();
    }
}
