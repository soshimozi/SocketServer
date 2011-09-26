using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.Crypto;
using SocketService.Framework.Client.Sockets;
using SocketService.Framework.Client.Response;
using System.Net.Sockets;
using System.Threading;
using SocketService.Framework.Client.Serialize;
using System.Collections;
using SocketService.Framework.Request;
using SocketService.Framework.SharedObjects;
using SocketService.Client.API.Command;
using SocketService.Framework.Client.Event;
using SocketService.Client.API.Event;
using SocketService.Client.API.Manager;

namespace SocketService.Client.API
{
    public class ClientEngine
    {
        //private ManualResetEvent _stopEvent = new ManualResetEvent(false);
        //private ManualResetEvent _serverDisconnectedEvent = new ManualResetEvent(false);

        //private DiffieHellmanKey _remotePublicKey;
        //private DiffieHellmanProvider _provider;

        //CommandQueue commandQueue = new CommandQueue();


        //private Queue<object> _outboundQueue = new Queue<object>();
        //private Queue<object> _inboundQueue = new Queue<object>();

        //private object _inboundQueueLock = new object();
        //private object _outboundQueueLock = new object();

        //private ZipSocket socket;

        public event EventHandler<ServerMessageReceivedArgs> ServerMessageRecieved;
        public event EventHandler<LoginResponseEventArgs> LoginResponseReceived;
        public event EventHandler<GetRoomVariableResponseArgs> GetRoomVariableResponseRecieved;
        public event EventHandler<JoinRoomEventArgs> JoinRoom;
        public event EventHandler<RoomUserUpdateEventArgs> RoomUserUpdate;
        public event EventHandler<LeaveRoomEventArgs> LeaveRoom;
        public event EventHandler<RoomVariableUpdateArgs> RoomVariableUpdate;

        private readonly List<Server> _servers = new List<Server>();

        public ClientEngine()
        {
            Managers = new ManagerHelper();
        }

        private ManagerHelper _helper;
        public ManagerHelper Managers
        {
            get { return _helper;  }
            private set
            {
                _helper = value;
                _helper.ClientEngine = this;
            }

        }

        public void AddServer(Server server)
        {
            lock (_servers)
            { _servers.Add(server); }

            server.ServerEvent += new EventHandler<ServerEventEventArgs>(server_ServerEvent);
            server.ServerResponse += new EventHandler<ServerResponseEventArgs>(server_ServerResponse);

        }

        void server_ServerResponse(object sender, ServerResponseEventArgs e)
        {
            HandleServerResponse(e.Response);
        }

        void server_ServerEvent(object sender, ServerEventEventArgs e)
        {
            HandleEvent(e.ServerEvent);
        }

        private void HandleServerResponse(IResponse response)
        {
            if( response is GetRoomVariableResponse)
            {
                OnGetRoomVariableResponseRecieved(
                    new GetRoomVariableResponseArgs()
                    {
                        Response = response as GetRoomVariableResponse
                    }
                );
            }
            else if (response is LoginResponse)
            {
                OnLoginResponseReceieved(
                    new LoginResponseEventArgs()
                    {
                        LoginResponse = response as LoginResponse
                    }
                );
            }
        }

        private void HandleEvent(IEvent evt)
        {
            if (evt is JoinRoomEvent)
            {
                OnJoinRoomEvent(
                    new JoinRoomEventArgs()
                    {
                        Event = evt as JoinRoomEvent
                    }
                );
            }
            else if (evt is RoomUserUpdateEvent)
            {
                OnRoomUserUpdate(
                    new RoomUserUpdateEventArgs()
                    {
                        Event = evt as RoomUserUpdateEvent
                    }
               );
            }
            else if (evt is RoomVariableUpdateEvent)
            {
                OnRoomVariableUpdate(
                    new RoomVariableUpdateArgs()
                    {
                        Event = evt as RoomVariableUpdateEvent
                    }
                );

            }
        }

        private void OnRoomVariableUpdate(RoomVariableUpdateArgs args)
        {
            var func = RoomVariableUpdate;
            if (func != null)
            {
                func(this, args);
            }
        }

        protected virtual void OnLeaveRoom(LeaveRoomEventArgs args)
        {
            var func = LeaveRoom;
            if (func != null)
            {
                func(this, args);
            }
        }
    

        protected virtual void OnRoomUserUpdate(RoomUserUpdateEventArgs args)
        {
            var func = RoomUserUpdate;
            if (func != null)
            {
                func(this, args);
            }
        }

        //protected virtual void OnConnectionResponse(ConnectionResponseEventArgs args)
        //{
        //    var func = ConnectionResponse;
        //    if (func != null)
        //    {
        //        func(this, args);
        //    }
        
        //}
    
        //protected virtual void OnListUsersInRoomResponseReceived(ListUsersInRoomResponseArgs args)
        //{
        //    var func = ListUsersInRoomResponseReceived;
        //    if (func != null)
        //    {
        //        func(this, args);
        //    }
        //}

        protected virtual void OnGetRoomVariableResponseRecieved(GetRoomVariableResponseArgs args)
        {
            var func = GetRoomVariableResponseRecieved;
            if (func != null)
            {
                func(this, args);
            }
        }
    
        protected virtual void OnServerMessageReceived(ServerMessageReceivedArgs args)
        {
            var func = ServerMessageRecieved;
            if (func != null)
            {
                func(this, args);
            }
        }
    
        protected virtual void OnLoginResponseReceieved(LoginResponseEventArgs args)
        {
            var func = LoginResponseReceived;
            if (func != null)
            {
                func(this, args);
            }
        }

        public void StopEngine()
        {
            lock (_servers)
            {
                foreach (Server server in _servers)
                {
                    server.Disconnect();
                }
            }
        }
    
        /// <summary>
        /// Connects this instance.
        /// </summary>
        //public void Connect()
        //{
        //    if (!_connected)
        //    {
        //        Thread serverThread = new Thread(new ThreadStart(ServerThread));
        //        serverThread.Start();
        //    }
        //}

        //protected virtual void ServerThread()
        //{
        //    // TODO: read info from configuration
        //    Socket rawSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //    bool connected = false;
        //    while (!connected)
        //    {
        //        try
        //        {
        //            rawSocket.Connect("127.0.0.1", 4000);
        //            connected = true;
        //        }
        //        catch (SocketException)
        //        {
        //            OnConnectionResponse(new ConnectionResponseEventArgs() { IsSuccessful = false });
        //            return;
        //        }
        //    }

        //    // wrap the socket
        //    socket = new ZipSocket(rawSocket, Guid.NewGuid());
        //    NegotiateKeys();

        //    OnConnectionResponse(new ConnectionResponseEventArgs() { IsSuccessful = true });
        //    _connected = true;

        //    //Thread messageThread = new Thread(new ThreadStart(PumpMessages));
        //    //messageThread.Start();
        //    PumpMessages();
        //}
    
        //public void Login(string loginName)
        //{
        //    // send login response
        //    LoginRequest request = new LoginRequest();
        //    request.LoginName = loginName;

        //    SendRequest(CreateRequest(EncryptionType.AES, request));
        //}

        //private IRequest CreateRequest(EncryptionType encryptionType, object requestData)
        //{
        //    if (encryptionType != EncryptionType.None)
        //    {
        //        using (Wrapper cryptoWrapper = 
        //            Wrapper.CreateEncryptor(AlgorithmType.TripleDES, 
        //                    _provider.CreatePrivateKey(_remotePublicKey).ToByteArray()))
        //        {
        //            return new ClientRequest(cryptoWrapper.IV,
        //                   EncryptionType.TripleDES, 
        //                   DateTime.Now, 0, 
        //                   cryptoWrapper.Encrypt(ObjectSerialize.Serialize(requestData)));
        //        }
        //    }
        //    else
        //    {
        //        return new ClientRequest(new byte[0] { },
        //                EncryptionType.None, 
        //                DateTime.Now, 0, 
        //                ObjectSerialize.Serialize(requestData));
        //    }
        //}
    

        //private void NegotiateKeys()
        //{
        //    SendRequest(CreateRequest(EncryptionType.None, new GetCentralAuthorityRequest()));

        //    bool doneHandshaking = false;
        //    int step = 0;

        //    // wait for central authority
        //    IList readList = new List<Socket>() { socket.RawSocket };
        //    while (!doneHandshaking)
        //    {
        //        Socket.Select(readList, null, null, -1);

        //        // there is only one socket in the poll list
        //        // so if the count is greater than 0 then
        //        // the only one available should be the client socket
        //        if (readList.Count > 0)
        //        {
        //            int availableBytes = socket.RawSocket.Available;
        //            if (availableBytes > 0)
        //            {
        //                byte[] objectData = socket.ReceiveData();
        //                switch (step)
        //                {
        //                    case 0:
        //                        CentralAuthority ca = ObjectSerialize.Deserialize<CentralAuthority>(objectData);
        //                        if (ca != null)
        //                        {
        //                            _provider = ca.GetProvider();

        //                            // Send Negotiate Key Command
        //                            // Read Response when it comes back
        //                            SendRequest(CreateRequest(EncryptionType.None,
        //                                new NegotiateKeysRequest(_provider.PublicKey.ToByteArray())));

        //                            step++;
        //                        }
        //                        break;

        //                    case 1:
        //                        // record server key
        //                        NegotiateKeysResponse response = ObjectSerialize.Deserialize<NegotiateKeysResponse>(objectData);
        //                        if (response != null)
        //                        {
        //                            _remotePublicKey = _provider.Import(response.RemotePublicKey);
        //                            doneHandshaking = true;
        //                        }
        //                        break;

        //                }
        //            }
        //        }
        //    }
        //}

        //private void SendRequest(IRequest request)
        //{
        //    socket.SendData(ObjectSerialize.Serialize(request));
        //}

        //public void Send(object data)
        //{
        //    SendRequest(CreateRequest(EncryptionType.AES, data));
        //}

        //protected void PumpMessages()
        //{
        //    bool serverDown = false;

        //    while (!serverDown && !_serverDisconnectedEvent.WaitOne(0))
        //    {
        //        IList readList = new List<Socket>() { socket.RawSocket };

        //        // now let's wait for messages
        //        Socket.Select(readList, null, null, 100);

        //        object message = null;

        //        // there is only one socket in the poll list
        //        // so if the count is greater than 0 then
        //        // the only one available should be the client socket
        //        if (readList.Count > 0)
        //        {
        //            // if socket is selected, and if available byes is 0, 
        //            // then socket has been closed
        //            serverDown = socket.RawSocket.Available == 0;
        //            if (!serverDown)
        //            {
        //                byte[] objectData = socket.ReceiveData();

        //                // it should be a server message, we can look
        //                // at other message types later
        //                message = ObjectSerialize.Deserialize(objectData);

        //                HandleMessage(message);
        //            }

        //        }
        //    }

        //    _serverDisconnectedEvent.Set();
        //}

        //private void HandleMessage(object message)
        //{
        //    //if (message.GetType() == typeof(ListUsersInRoomResponse))
        //    //{
        //    //    OnListUsersInRoomResponseReceived(
        //    //        new ListUsersInRoomResponseArgs()
        //    //        {
        //    //            Response = (ListUsersInRoomResponse)message
        //    //        }
        //    //    );
        //    //}
        //    //else 
        //    if (message.GetType() == typeof(GetRoomVariableResponse))
        //    {
        //        OnGetRoomVariableResponseRecieved(
        //            new GetRoomVariableResponseArgs() 
        //            { 
        //                Response = (GetRoomVariableResponse)message 
        //            }
        //        );
        //    }
        //    //else if (message.GetType() == typeof(ServerMessage))
        //    //{
        //    //    OnServerMessageReceived( 
        //    //        new ServerMessageReceivedArgs() 
        //    //        { 
        //    //            Message = ((ServerMessage)message).Message 
        //    //        }
        //    //    );
        //    //}
        //    else if (message.GetType() == typeof(LoginResponse))
        //    {
        //        OnLoginResponseReceieved(
        //            new LoginResponseEventArgs() 
        //            { 
        //                LoginResponse = (LoginResponse)message 
        //            }
        //        );
        //    }
        //    else if (message.GetType() == typeof(JoinRoomEvent))
        //    {
        //        OnJoinRoomEvent(
        //            new JoinRoomEventArgs()
        //            {
        //                Event = message as JoinRoomEvent
        //            }
        //        );
        //    }
        //    else if (message.GetType() == typeof(RoomUserUpdateEvent))
        //    {
        //        OnRoomUserUpdate(
        //            new RoomUserUpdateEventArgs() 
        //            {
        //                Event = message as RoomUserUpdateEvent
        //            }
        //       );
        //    }
        //}

        protected virtual void OnJoinRoomEvent(JoinRoomEventArgs args)
        {
            var func = JoinRoom;
            if (func != null)
            {
                func(this, args);
            }

        }

    }
}
