using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SocketServer
{
    public interface ICommunicationTransport
    {
        void Connect();
        void Disconnect(bool force);
        bool IsConnected { get; }
        bool HasConnection { get; }
        Stream Stream { get; }

        int SendTimeout { get; set; }
        int ReceiveTimeout { get; set; }
    }
}
