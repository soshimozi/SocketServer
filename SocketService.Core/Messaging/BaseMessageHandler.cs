using System;

namespace SocketService.Core.Messaging
{
    [Serializable]
    public abstract class BaseMessageHandler : ICommand
    {
        public abstract void Execute();
    }
}
