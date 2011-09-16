using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Messaging
{
    [Serializable]
    public abstract class BaseCommand : ICommand
    {
        public abstract void Execute();
    }
}
