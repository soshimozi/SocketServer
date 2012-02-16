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

namespace SocketServer.Net
{
    public class SocketServer
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly object clientSync = new object();
        //private readonly Mutex _clientListLock = new Mutex();

        private readonly Dictionary<Guid, ClientConnection> _connectionList = new Dictionary<Guid, ClientConnection>();
        private readonly ManualResetEvent _stopEvent = new ManualResetEvent(false);
        //private Socket _listenSocket;

        //private readonly IPAddress address = IPAddress.Any;
        //private TcpListener listener;
        private bool _stopped = true;

        private readonly INetworkListener listener;
        public SocketServer(IPAddress address)
        {
            listener = new SocketListener(address);
        }

        public SocketServer()
        {
            listener = new SocketListener();
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
        //{
        //    add
        //    {
        //        lock (clientSync)
        //        {
        //            foreach (ClientConnection connection in _connectionList.Values)
        //                connection.MessageReceived += value;
        //        }
        //    }
        //    remove
        //    {
        //        lock (clientSync)
        //        {
        //            foreach (ClientConnection connection in _connectionList.Values)
        //                connection.MessageReceived -= value;
        //        }
        //    }
        //}

        ///// <summary>
        ///// Event Raised whenever an outgoing message is sent
        ///// </summary>
        //public event EventHandler<MessageEventArgs> MessageSent
        //{
        //    add
        //    {
        //        lock (clientSync)
        //        {
        //            foreach (ClientConnection channel in clients)
        //                channel.MessageSent += value;
        //        }
        //    }
        //    remove
        //    {
        //        lock (clientSync)
        //        {
        //            foreach (ClientConnection channel in clients)
        //                channel.MessageSent -= value;
        //        }
        //    }
        //}

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
        public void DisconnectClient(Guid clientId)
        {
            OnClientDisconnected(this, new DisconnectedArgs() { ClientId = clientId });
            //lock (clientSync)
            //{
            //    try
            //    {
            //        if (_connectionList.ContainsKey(clientId))
            //        {
            //            _connectionList[clientId].ClientSocket.Close();
            //            OnClientDisconnected(clientId);

            //            _connectionList.Remove(clientId);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Log.ErrorFormat("Error: {0}", ex.Message);
            //    }
            //}
        }

        //protected virtual void OnDataRecieved(Guid clientId, byte[] data)
        //{
        //    EventHandler<DataRecievedArgs> dataRecieved = DataRecieved;
        //    if (dataRecieved != null)
        //    {
        //        var args = new DataRecievedArgs(clientId, data);
        //        dataRecieved(this, args);
        //    }
        //}

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
                    if (_connectionList.ContainsKey(args.ClientId))
                    {
                        _connectionList.Remove(args.ClientId);
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
                        //OnClientDisconnected(this, key);
                    }

                    //_connectionList.Clear();
                }
                catch (Exception ex)
                {
                    Log.ErrorFormat("Error: {0}", ex.Message);
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

                        //DHParametersGenerator generator = new DHParametersGenerator();
                        //generator.Init(256, 30, new SecureRandom());
                        //var sharedParameters = generator.GenerateParameters();

                        //IAsymmetricCipherKeyPairGenerator keyGen = GeneratorUtilities.GetKeyPairGenerator("DH");
                        //keyGen.Init(new DHKeyGenerationParameters(new SecureRandom(), sharedParameters));

                        //var keyPair = keyGen.GenerateKeyPair();

                        ClientConnection channel = ClientConnection.CreateClientConnection(
                            new PlainEnvelope(), 
                            client);

                        channel.ClientId = Guid.NewGuid();

                        Log.Info("New connection from : " + client.RemoteEndPoint);

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
                    Log.Debug("SocketServer shutdown.  No longer accepting connections");
                }
            }

            //var port = (int) threadParam;

            //listener = new TcpListener(port);

            //var localEndPoint = new IPEndPoint(IPAddress.Any, port);
            //_listenSocket.Bind(localEndPoint);
            //_listenSocket.Listen(30);

            //_listenSocket.BeginAccept(OnBeginAccept, null);

            //var pollThread = new Thread(Poll);
            //pollThread.Start();
        }

        protected void OnMessageReceived(object sender, MessageEventArgs args)
        {
            EventHandler<MessageEventArgs> function = MessageReceived;
            if (function != null)
            {
                function(sender, args);
            }
        }

        //private void OnBeginAccept(IAsyncResult result)
        //{
        //    if (result.IsCompleted)
        //    {
        //        Socket socket = null;

        //        try
        //        {
        //            socket = _listenSocket.EndAccept(result);
        //        }
        //        catch (Exception ex)
        //        {
        //            Log.ErrorFormat("Error: {0}", ex.Message);
        //        }

        //        _listenSocket.BeginAccept(OnBeginAccept, null);

        //        if (socket != null)
        //        {
        //            Guid clientId = Guid.NewGuid();
        //            var client = new ZipSocket(socket);

        //            ClientConnection connection = ClientConnection.CreateClientConnection(new EncryptedMessageEnvelope(), new ZipSocket(socket));
        //            OnClientConnected(clientId, connection, ((IPEndPoint) socket.RemoteEndPoint).Address.ToString());
        //            AddConnection(clientId, connection);
        //        }
        //    }
        //    else
        //    {
        //        _listenSocket.BeginAccept(OnBeginAccept, null);
        //    }
        //}

        /// <summary>
        /// Retrieves the guid (clientId) associated with this socket
        /// </summary>
        /// <param name="socket" />
        /// <param name="clientId" />
        /// <returns />
        //private ClientConnection FindClientBySocket(Socket socket, out Guid clientId)
        //{
        //    //clientId = Guid.Empty;

        //    //lock (clientSync)
        //    //{
        //    //    try
        //    //    {
        //    //        IEnumerable<KeyValuePair<Guid, ClientConnection>> query = from li in _connectionList
        //    //                                                           where li.Value.Transport. == socket
        //    //                                                           select li;

        //    //        KeyValuePair<Guid, ClientConnection> kvp = query.FirstOrDefault();
        //    //        clientId = kvp.Key;
        //    //        return kvp.Value;
        //    //    }
        //    //    catch (Exception ex)
        //    //    {
        //    //        Log.ErrorFormat("Error: {0}", ex.Message);
        //    //        return null;
        //    //    }
        //    //}

        //    return null;
        //}

        //private void Poll()
        //{
        //    while (!_stopEvent.WaitOne(20))
        //    {
        //        IList readList = BuildSocketList();
        //        if (readList.Count > 0)
        //        {
        //            Socket.Select(readList, null, null, 0);
        //            ProcessSelectedSockets(readList);
        //        }
        //    }

        //    //_listenSocket.Shutdown(SocketShutdown.Both);
        //    listener.Stop();
        //    //_listenSocket.Close();

        //    IsStopped = true;
        //}

        //private void ProcessSelectedSockets(IList readList)
        //{
        //    foreach (object listObject in readList)
        //    {
        //        var socket = listObject as Socket;
        //        if (socket != null && !IsStopped)
        //        {
        //            Guid clientId;
        //            ClientConnection client = FindClientBySocket(socket, out clientId);
        //            if (client != null)
        //            {
        //                if (socket.Connected)
        //                {
        //                    int availableBytes = socket.Available;

        //                    if (availableBytes > 0)
        //                    {
        //                        var data = new byte[availableBytes];
        //                        socket.Receive(data);
        //                        //OnDataRecieved(clientId, data);
        //                    }
        //                    else
        //                    {
        //                        DisconnectClient(clientId);
        //                    }
        //                }
        //                else
        //                {
        //                    //log.Debug(string.Format("SocketServer.ProcessReadList => Zombie socket, client id {0}", FindClientIdForSocket(socket)) );
        //                    DisconnectClient(clientId);
        //                }
        //            }
        //        }
        //    }
        //}

        //private List<Socket> BuildSocketList()
        //{
        //    var clientList = new List<Socket>();

        //    Socket[] socketArray = null;

        //    lock (clientSync)
        //    {
        //        try
        //        {
        //            socketArray = new Socket[_connectionList.Count];

        //            IEnumerable<Socket> q = from nvp in _connectionList
        //                                    select nvp.Value.ClientSocket.RawSocket;

        //            q.ToArray().CopyTo(socketArray, 0);
        //        }
        //        catch (Exception ex)
        //        {
        //            Log.ErrorFormat("Error: {0}", ex.Message);
        //        }
        //    }

        //    if (socketArray != null)
        //    {
        //        clientList.AddRange(socketArray);
        //    }

        //    return clientList;
        //}

        //private void AddConnection(Guid clientId, ClientConnection clientConnection)
        //{
        //    lock (clientSync)
        //    {
        //        try
        //        {
        //            _connectionList.Add(clientId, clientConnection);
        //        }
        //        catch (Exception ex)
        //        {
        //            Log.ErrorFormat("Error: {0}", ex.Message);
        //        }
        //    }
        //}
    }
}