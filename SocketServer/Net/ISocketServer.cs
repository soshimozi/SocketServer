using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Net
{
    public interface ISocketServer
    {
        void StartServer(int serverPort);
        void StopServer();
    }
}
