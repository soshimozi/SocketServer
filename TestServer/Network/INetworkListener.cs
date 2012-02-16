using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestServer.Network
{
    public interface INetworkListener
    {
        void Initialize(int port);

        INetworkTransport AcceptClient(int timeout);

        void Start();

        void Stop();
    }
}
