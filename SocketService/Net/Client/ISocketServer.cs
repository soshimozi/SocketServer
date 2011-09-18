using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace SocketService.Sockets
{
    public interface ISocketServer
    {
        void DisconnectClient(Guid key);
        void SendData(Guid key, byte[] buffer, int offset, int count);
    }
}
