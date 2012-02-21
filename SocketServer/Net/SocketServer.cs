using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using SocketServer.Net.Client;
using SocketServer.Shared.Sockets;
using log4net;
using SocketServer.Shared;
using SocketServer.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using SocketServer.Shared.Messaging;
using SocketServer.Shared.Network;
using Google.ProtocolBuffers.DescriptorProtos;
using System.IO;

namespace SocketServer.Net
{
    public class SocketServer
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly object clientSync = new object();
        //private readonly Mutex _clientListLock = new Mutex();

        private readonly Dictionary<Guid, ClientConnection> _connectionList = new Dictionary<Guid, ClientConnection>();
        private readonly ManualResetEvent _stopEvent = new ManualResetEvent(false);

        private bool _stopped = true;

        private readonly INetworkListener listener;

        private readonly MessageRegistry registry;

        public SocketServer(IPAddress address, MessageRegistry registry)
        {
            listener = new SocketListener(address);
            this.registry = registry;
        }

        public SocketServer(MessageRegistry registry)
        {
            listener = new SocketListener();
            this.registry = registry;
        }


        /// <summary>
        /// Gets or sets a value indicating whether this instance is stopped.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is stopped; otherwise, <c>false</c>.
        /// </value>
        public bool IsStopped
        {
            get
            {
                Thread.MemoryBarrier();
                return _stopped;
            }

            set
            {
                _stopped = value;
                Thread.MemoryBarrier();
            }
        }

        #region Events

        /// <summary>
        /// Raised when a Channel has disconnected remotely
        /// </summary>
        public event EventHandler<ConnectArgs> ClientConnected;

        /// <summary>
        /// Raised when a new incoming Channel has established a connection.
        /// </summary>
        public event EventHandler<DisconnectedArgs> ClientDisconnected;

        /// <summary>
        /// Event Raised whenever an incoming message is received
        /// </summary>
        public event EventHandler<MessageEventArgs> MessageReceived;

        ///// <summary>
        ///// Event Raised whenever an outgoing message is sent
        ///// </summary>
        //public event EventHandler<MessageEventArgs> MessageSent;


        #endregion Events

        public void StartServer(int serverPort)
        {
            if (IsStopped)
            {
                _stopEvent.Reset();

                IsStopped = false;

                listener.Initialize(serverPort);

                var serverThread = new Thread(new ThreadStart(ServerMain));
                serverThread.Start();
            }
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        public void StopServer()
        {
            _stopEvent.Set();

            IsStopped = true;
            DisconnectAllClients();
        }

        /// <summary>
        /// Disconnects the client.
        /// </summary>
        /// <param name="clientId"></param>
        public void DisconnectClient(ClientConnection connection)
        {
            connection.Disconnect();

            OnClientDisconnected(this, new DisconnectedArgs() { Connection = connection });
        }

        protected virtual void OnClientConnected(Guid clientId, ClientConnection connection, string remoteAddress)
        {
            EventHandler<ConnectArgs> clientConnected = ClientConnected;
            if (clientConnected != null)
            {
                var args = new ConnectArgs(clientId, connection, remoteAddress);
                clientConnected(this, args);
            }
        }

        protected virtual void OnClientDisconnected(object sender, DisconnectedArgs args)
        {
            lock (clientSync)
            {
                // remove channel from our internal list
                for (int i = 0; i < _connectionList.Count; i++)
                {
                    if (_connectionList.ContainsKey(args.Connection.ClientId))
                    {
                        var connection = _connectionList[args.Connection.ClientId];
                        _connectionList.Remove(args.Connection.ClientId);
                        break;
                    }
                }

                EventHandler<DisconnectedArgs> handler = ClientDisconnected;
                if (handler != null)
                    handler(this, args);
            }

        }

        private void DisconnectAllClients()
        {
            //lock (clientSync)
            {
                try
                {
                    foreach (Guid key in _connectionList.Keys)
                    {
                        _connectionList[key].Transport.Disconnect(true);
                    }
                }
                catch (Exception ex)
                {
                    Logger.ErrorFormat("Error: {0}", ex.Message);
                }
            }
        }

        private void ServerMain()
        {
            listener.Start();

            while (!IsStopped)
            {
                try
                {
                    INetworkTransport client = listener.AcceptClient();

                    if (client != null)
                    {
                        ClientConnection channel = ClientConnection.CreateClientConnection(
                            new ProtoBuffEnvelope(registry),
                            client);

                        channel.ClientId = Guid.NewGuid();

                        Logger.Info("New connection from : " + client.RemoteEndPoint);

                        lock (clientSync)
                        {
                            _connectionList.Add(channel.ClientId, channel);
                        }

                        OnClientConnected(channel.ClientId, channel, client.RemoteEndPoint.ToString());

                        channel.ClientClosed += OnClientDisconnected;
                        channel.MessageReceived += OnMessageReceived;
                    }
                }
                catch (SocketException)
                {
                    // This is here because AcceptTcpClient throws an exception when we tell it
                    // stop listening.
                    Logger.Debug("SocketServer shutdown.  No longer accepting connections");
                }
            }

        }

        protected void OnMessageReceived(object sender, MessageEventArgs args)
        {
            EventHandler<MessageEventArgs> function = MessageReceived;
            if (function != null)
            {
                function(sender, args);
            }
        }

    }
}