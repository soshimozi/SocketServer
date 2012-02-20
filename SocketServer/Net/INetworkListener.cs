using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketServer
{
    public interface INetworkListener
    {
        void Initialize(int port);

        INetworkTransport AcceptClient();

        void Start();

        void Stop();
    }
}
