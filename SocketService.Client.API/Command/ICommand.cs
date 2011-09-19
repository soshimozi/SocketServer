using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Client.API.Command
{
    interface ICommand
    {
        void Execute();
    }
}
