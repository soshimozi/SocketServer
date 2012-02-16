using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SocketServer
{
    public class SocketListener : INetworkListener
    {
        #region Fields
        readonly IPAddress address = IPAddress.Any;
        private TcpListener listener;

        private volatile bool listening;

        private TcpClient newClient;
        private readonly AutoResetEvent newConnection = new AutoResetEvent(false);
        private readonly AutoResetEvent shutdown = new AutoResetEvent(false);
        #endregion

        #region Constructors
        public SocketListener()
        {
        }

        public SocketListener(IPAddress address)
        {
            this.address = address;
        }
        #endregion

        public void Initialize(int port)
        {
            listener = new TcpListener(address, port);
        }
        public void Start()
        {
            listener.Start();
            listening = true;
        }

        public void Stop()
        {
            listening = false;
            listener.Stop();
            shutdown.Set();
        }
        public INetworkTransport AcceptClient()
        {
            newClient = null;
            // perform all accept calls asynchronously            
            listener.BeginAcceptTcpClient(WaitForConnection, null);

            // block until a connection is accepted, OR the shutdown event is given
            int index = WaitHandle.WaitAny(new WaitHandle[] { shutdown, newConnection });
            if (index == 1 && newClient != null)
            {
                return new SocketTransport(newClient);
            }
            throw new SocketException();
        }

        private void WaitForConnection(IAsyncResult ar)
        {
            try
            {
                newClient = listener.EndAcceptTcpClient(ar);    
            }
            catch(ObjectDisposedException)
            {
                if (!listening) return;

                throw;
            }
            
            newConnection.Set();
        }
    }
}
