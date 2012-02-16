using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketServer.Shared.Network;

namespace SocketServer.Handler
{
    public interface IRequestHandler<T>
    {
        void HandleRequest(T request, ClientConnection connection);
    }
}
