using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace SocketServer.Net
{
    class SocketListener : INetworkListener
    {
        readonly IPAddress address = IPAddress.Any;
        private TcpListener listener;

        private volatile bool running;
        private readonly AutoResetEvent shutdown = new AutoResetEvent(false);

        public SocketListener()
        {

        }

        public SocketListener(IPAddress address)
        {
            this.address = address;
        }

        public void Initialize(int port)
        {
            listener = new TcpListener(address, port);
        }

        public INetworkTransport AcceptClient()
        {
            // use polling to accept a client
            while (!shutdown.WaitOne(100))
            {
                List<Socket> socketList = new List<Socket> { listener.Server };
                Socket.Select(socketList, null, null, 100);

                if (socketList.Count == 1)
                {
                    // we have an accept waiting
                    TcpClient client = listener.AcceptTcpClient();
                    if (client != null)
                    {
                        return new SocketTransport(client);
                    }
                }
            }

            return null;
        }

        public void Start()
        {
            listener.Start();
            running = true;

        }

        public void Stop()
        {
            running = false;
            listener.Stop();
            shutdown.Set();
        }
    }
}
