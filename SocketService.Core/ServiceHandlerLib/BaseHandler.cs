using System;

namespace SocketServer.Core.ServiceHandlerLib
{
    [Serializable]
    public abstract class BaseHandler<T, TState> : IServiceHandler where T : class
    {
        public bool HandleRequest(object request, object state)
        {
            return HandleRequest(request as T, (TState)state);
        }

        public abstract bool HandleRequest(T request, TState state);
    }
}
