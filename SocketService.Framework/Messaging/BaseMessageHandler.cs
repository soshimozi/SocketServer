using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Framework.Messaging
{
    [Serializable]
    public abstract class BaseMessageHandler : ICommand
    {
        public abstract void Execute();
    }
}
