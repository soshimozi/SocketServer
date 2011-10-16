using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using SocketService.Framework.Client.Sockets;
using SocketService.Net.Client;
using log4net;

namespace SocketService.Net
{
    public class SocketServer
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Mutex _clientListLock = new Mutex();

        private readonly Dictionary<Guid, ZipSocket> _connectionList = new Dictionary<Guid, ZipSocket>();
        private readonly ManualResetEvent _stopEvent = new ManualResetEvent(false);
        private Socket _listenSocket;
        private bool _stopped = true;

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

        public event EventHandler<ConnectArgs> ClientConnecting;
        public event EventHandler<DataRecievedArgs> DataRecieved;
        public event EventHandler<DisconnectedArgs> ClientDisconnecting;

        /// <summary>
        /// Starts the server.
        /// </summary>
        /// <param name="serverPort">The server port.</param>
        public void StartServer(int serverPort)
        {
            if (IsStopped)
            {
                _stopEvent.Reset();

                IsStopped = false;

                var serverThread = new Thread(ServerMain);
                serverThread.Start(serverPort);
            }
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        public void StopServer()
        {
            _stopEvent.Set();
            DisconnectAllClients();
        }

        /// <summary>
        /// Disconnects the client.
        /// </summary>
        /// <param name="clientId"></param>
        public void DisconnectClient(Guid clientId)
        {
            _clientListLock.WaitOne();
            try
            {
                if (_connectionList.ContainsKey(clientId))
                {
                    _connectionList[clientId].Close();
                    OnClientDisconnected(clientId);

                    _connectionList.Remove(clientId);
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}", ex.Message);
            }
            finally
            {
                _clientListLock.ReleaseMutex();
            }
        }

        protected virtual void OnDataRecieved(Guid clientId, byte[] data)
        {
            EventHandler<DataRecievedArgs> dataRecieved = DataRecieved;
            if (dataRecieved != null)
            {
                var args = new DataRecievedArgs(clientId, data);
                dataRecieved(this, args);
            }
        }

        protected virtual void OnClientConnected(Guid clientId, Socket socket, string remoteAddress)
        {
            EventHandler<ConnectArgs> clientConnected = ClientConnecting;
            if (clientConnected != null)
            {
                var args = new ConnectArgs(clientId, socket, remoteAddress);
                clientConnected(this, args);
            }
        }

        protected virtual void OnClientDisconnected(Guid clientId)
        {
            EventHandler<DisconnectedArgs> clientDisconnected = ClientDisconnecting;
            if (clientDisconnected != null)
            {
                var args = new DisconnectedArgs(clientId);
                clientDisconnected(this, args);
            }
        }

        private void DisconnectAllClients()
        {
            _clientListLock.WaitOne();
            try
            {
                foreach (Guid key in _connectionList.Keys)
                {
                    _connectionList[key].Close();
                    OnClientDisconnected(key);
                }

                _connectionList.Clear();
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}", ex.Message);
            }
            finally
            {
                _clientListLock.ReleaseMutex();
            }
        }

        private void ServerMain(object threadParam)
        {
            var port = (int) threadParam;

            _listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var localEndPoint = new IPEndPoint(IPAddress.Any, port);
            _listenSocket.Bind(localEndPoint);
            _listenSocket.Listen(30);

            _listenSocket.BeginAccept(OnBeginAccept, null);

            var pollThread = new Thread(Poll);
            pollThread.Start();
        }

        private void OnBeginAccept(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                Socket socket = null;

                try
                {
                    socket = _listenSocket.EndAccept(result);
                }
                catch (Exception ex)
                {
                    Log.ErrorFormat("Error: {0}", ex.Message);
                }

                if (socket != null)
                {
                    Guid clientId = Guid.NewGuid();
                    var client = new ZipSocket(socket, clientId);

                    AddConnection(clientId, client);
                    OnClientConnected(clientId, socket, ((IPEndPoint) socket.RemoteEndPoint).Address.ToString());

                    _listenSocket.BeginAccept(OnBeginAccept, null);
                }
            }
            else
            {
                _listenSocket.BeginAccept(OnBeginAccept, null);
            }
        }

        /// <summary>
        /// Retrieves the guid (clientId) associated with this socket
        /// </summary>
        /// <param name="socket" />
        /// <param name="clientId" />
        /// <returns />
        private ZipSocket FindClientBySocket(Socket socket, out Guid clientId)
        {
            clientId = Guid.Empty;

            _clientListLock.WaitOne();
            try
            {
                IEnumerable<KeyValuePair<Guid, ZipSocket>> query = from li in _connectionList
                                                                   where li.Value.RawSocket == socket
                                                                   select li;


                KeyValuePair<Guid, ZipSocket> kvp = query.FirstOrDefault();
                clientId = kvp.Key;
                return kvp.Value;
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}", ex.Message);
                return null;
            }
            finally
            {
                _clientListLock.ReleaseMutex();
            }
        }

        private void Poll()
        {
            while (!_stopEvent.WaitOne(20))
            {
                IList readList = BuildSocketList();
                if (readList.Count > 0)
                {
                    Socket.Select(readList, null, null, 0);
                    ProcessSelectedSockets(readList);
                }
            }

            //_listenSocket.Shutdown(SocketShutdown.Both);
            _listenSocket.Close();

            IsStopped = true;
        }

        private void ProcessSelectedSockets(IList readList)
        {
            foreach (object listObject in readList)
            {
                var socket = listObject as Socket;
                if (socket != null && !IsStopped)
                {
                    Guid clientId;
                    ZipSocket client = FindClientBySocket(socket, out clientId);
                    if (client != null)
                    {
                        if (socket.Connected)
                        {
                            int availableBytes = socket.Available;

                            if (availableBytes > 0)
                            {
                                OnDataRecieved(clientId, client.ReceiveData());
                            }
                            else
                            {
                                DisconnectClient(clientId);
                            }
                        }
                        else
                        {
                            //log.Debug(string.Format("SocketServer.ProcessReadList => Zombie socket, client id {0}", FindClientIdForSocket(socket)) );
                            DisconnectClient(clientId);
                        }
                    }
                }
            }
        }

        private List<Socket> BuildSocketList()
        {
            var clientList = new List<Socket>();

            Socket[] socketArray = null;

            _clientListLock.WaitOne();
            try
            {
                socketArray = new Socket[_connectionList.Count];

                IEnumerable<Socket> q = from nvp in _connectionList
                                        select nvp.Value.RawSocket;

                q.ToArray().CopyTo(socketArray, 0);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}", ex.Message);
            }
            finally
            {
                _clientListLock.ReleaseMutex();
            }

            if (socketArray != null)
            {
                clientList.AddRange(socketArray);
            }

            return clientList;
        }

        private void AddConnection(Guid clientId, ZipSocket clientSocket)
        {
            _clientListLock.WaitOne();

            try
            {
                _connectionList.Add(clientId, clientSocket);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}", ex.Message);
            }
            finally
            {
                _clientListLock.ReleaseMutex();
            }
        }
    }
}