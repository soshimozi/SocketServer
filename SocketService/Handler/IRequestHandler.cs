using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketServer.Handler
{
    public interface IRequestHandler<T>
    {
        void HandleRequest(T request, Guid clientId);
    }
}
