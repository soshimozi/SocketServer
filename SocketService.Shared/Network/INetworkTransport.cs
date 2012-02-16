using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace SocketServer.Shared.Network
{
    public interface INetworkTransport
    {
        string Address { get; set; }
        int Port { get; set; }

        EndPoint RemoteEndPoint { get; }

        void Connect();
        void Disconnect(bool force);
        bool IsConnected { get; }
        bool HasConnection { get; }
        Stream Stream { get; }

        int SendTimeout { get; set; }
        int ReceiveTimeout { get; set; }
    }
}
