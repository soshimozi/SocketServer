using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using log4net;
using System.IO;
using System.Net;

namespace SocketServer.Net
{
    class SocketTransport : INetworkTransport
    {
        TcpClient client;
        private static readonly ILog logger = LogManager.GetLogger(typeof(SocketTransport));

        public SocketTransport()
        {
        }

        internal SocketTransport(TcpClient client)
        {
            this.client = client;
        }

        public bool HasConnection { get { return false; } }
        public string Address { get; set; }
        public int Port { get; set; }

        public void Connect()
        {
            client = new TcpClient(Address, Port)
                {
                        SendTimeout = sendTimeout,
                        ReceiveTimeout = receiveTimeout
                };
        }

        public void Disconnect(bool force)
        {
            if (force) client.Close();
            else if (client.Connected) client.Client.Shutdown(SocketShutdown.Send);
        }

        public bool IsConnected
        {
            get
            {
                return client != null && client.Connected;
            }
        }

        public bool NoDelay
        {
            get
            {
                return client.NoDelay;
            }
            set
            {
                client.NoDelay = value;
            }
        }

        public Stream Stream
        {
            get { return client.GetStream(); }
        }

        public EndPoint RemoteEndPoint
        {
            get
            {
                return client.Client.RemoteEndPoint;
            }
        }

        private int sendTimeout;
        public int SendTimeout
        {
            get { return sendTimeout; }
            set 
            {
                sendTimeout = value; 
                if (client != null && client.Client != null && client.Connected) client.SendTimeout = value; 
            }
        }

        private int receiveTimeout;
        public int ReceiveTimeout
        {
            get { return receiveTimeout; }
            set
            {
                receiveTimeout = value;
                if (client != null && client.Client != null && client.Connected) client.ReceiveTimeout = value;
            }
        }
    }
}
