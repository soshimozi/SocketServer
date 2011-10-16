using System;

namespace SocketService.Framework.Messaging
{
    [Serializable]
    public abstract class BaseMessageHandler : ICommand
    {
        public abstract void Execute();
    }
}
