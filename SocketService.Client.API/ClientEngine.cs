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

namespace SocketService.Client.API
{
    public class ClientEngine
    {
        private ManualResetEvent _stopEvent = new ManualResetEvent(false);
        private ManualResetEvent _serverDisconnectedEvent = new ManualResetEvent(false);

        private DiffieHellmanKey _remotePublicKey;
        private DiffieHellmanProvider _provider;

        private Queue<object> _outboundQueue = new Queue<object>();
        private Queue<object> _inboundQueue = new Queue<object>();

        private object _inboundQueueLock = new object();
        private object _outboundQueueLock = new object();

        private ZipSocket socket;

        public event EventHandler<ServerMessageReceivedArgs> ServerMessageRecieved;
        public event EventHandler<LoginResponseEventArgs> LoginResponseReceived;
        public event EventHandler<GetRoomVariableResponseArgs> GetRoomVariableResponseRecieved;
        public event EventHandler<ListUsersInRoomResponseArgs> ListUsersInRoomResponseReceived;

        protected virtual void OnListUsersInRoomResponseReceived(ListUsersInRoomResponseArgs args)
        {
            var func = ListUsersInRoomResponseReceived;
            if (func != null)
            {
                func(this, args);
            }
        }

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
            _serverDisconnectedEvent.Set();
        }
    
        /// <summary>
        /// Connects this instance.
        /// </summary>
        public bool Connect()
        {
            // TODO: read info from configuration
            Socket rawSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            bool connected = false;
            while (!connected)
            {
                try
                {
                    rawSocket.Connect("127.0.0.1", 4000);
                    connected = true;
                }
                catch (SocketException)
                {
                    return false;
                }
            }

            // wrap the socket
            socket = new ZipSocket(rawSocket, Guid.NewGuid());
            NegotiateKeys();

            Thread sendMessagesThread = new Thread(new ThreadStart(SendMessages));
            sendMessagesThread.Start();

            Thread recieveMessagesThread = new Thread(new ThreadStart(ProcessMessages));
            recieveMessagesThread.Start();

            Thread messageThread = new Thread(new ThreadStart(PumpMessages));
            messageThread.Start();

            return true;
        }

        public void Login(string loginName)
        {
            // send login response
            LoginRequest request = new LoginRequest();
            request.LoginName = loginName;

            SendObject(EncryptionType.AES, request, false);
        }
    

        private void NegotiateKeys()
        {
            SendObject(EncryptionType.None, new GetCentralAuthorityRequest(), false);
            bool doneHandshaking = false;
            int step = 0;

            // wait for central authority
            IList readList = new List<Socket>() { socket.RawSocket };
            while (!doneHandshaking)
            {
                Socket.Select(readList, null, null, -1);

                // there is only one socket in the poll list
                // so if the count is greater than 0 then
                // the only one available should be the client socket
                if (readList.Count > 0)
                {
                    int availableBytes = socket.RawSocket.Available;
                    if (availableBytes > 0)
                    {
                        byte[] objectData = socket.ReceiveData();
                        switch (step)
                        {
                            case 0:
                                CentralAuthority ca = ObjectSerialize.Deserialize<CentralAuthority>(objectData);
                                if (ca != null)
                                {
                                    _provider = ca.GetProvider();

                                    // Send Negotiate Key Command
                                    // Read Response when it comes back
                                    SendObject(EncryptionType.None,
                                        new NegotiateKeysRequest(_provider.PublicKey.ToByteArray()), false);

                                    step++;
                                }
                                break;

                            case 1:
                                // record server key
                                NegotiateKeysResponse response = ObjectSerialize.Deserialize<NegotiateKeysResponse>(objectData);
                                if (response != null)
                                {
                                    _remotePublicKey = _provider.Import(response.RemotePublicKey);
                                    doneHandshaking = true;
                                }
                                break;

                        }
                    }
                }
            }
        }

        private void SendObject(EncryptionType encryptionType, object graph, bool sendLater)
        {
            ClientRequestHeader header;
            if (encryptionType != EncryptionType.None)
            {
                using (Wrapper cryptoWrapper = Wrapper.CreateEncryptor(AlgorithmType.TripleDES, _provider.CreatePrivateKey(_remotePublicKey).ToByteArray()))
                {
                    byte[] requestData = ObjectSerialize.Serialize(graph);

                    header = new ClientRequestHeader(cryptoWrapper.IV,
                           EncryptionType.TripleDES, DateTime.Now, 0, cryptoWrapper.Encrypt(requestData));
                }
            }
            else
            {
                header = new ClientRequestHeader(new byte[0] { },
                        encryptionType, DateTime.Now, 0, ObjectSerialize.Serialize(graph));
            }

            if( sendLater )
                AddOutboundMessage(header);
            else
                SendData(socket, ObjectSerialize.Serialize(header));
        }

        private void SendData(ZipSocket client, byte[] data)
        {
            client.SendData(data);
        }

        private bool ReadMessage(out ServerMessage message)
        {
            bool serverDown = false;

            IList readList = new List<Socket>() { socket.RawSocket };

            // now let's wait for messages
            Socket.Select(readList, null, null, 0);

            message = null;

            // there is only one socket in the poll list
            // so if the count is greater than 0 then
            // the only one available should be the client socket
            if (readList.Count > 0)
            {
                // if socket is selected, and if available byes is 0, 
                // then socket has been closed
                serverDown = socket.RawSocket.Available == 0;

                if (!serverDown)
                {
                    message = ReadServerMessage(socket);
                }

            }

            return serverDown;
        }

        private ServerMessage ReadServerMessage(ZipSocket socket)
        {
            int availableBytes = socket.RawSocket.Available;
            if (availableBytes > 0)
            {
                byte[] objectData = socket.ReceiveData();

                // it should be a server message, we can look
                // at other message types later
                return ObjectSerialize.Deserialize<ServerMessage>(objectData);
            }

            return null;
        }

        protected void SendMessages()
        {
            while (!_serverDisconnectedEvent.WaitOne(100))
            {
                object message = null;
                lock (_outboundQueueLock)
                {
                    if (_outboundQueue.Count > 0)
                    {
                        message = _outboundQueue.Dequeue();
                    }
                }

                if (message != null)
                {
                    SendData(socket, ObjectSerialize.Serialize(message));
                }
            }
        }

        protected void ProcessMessages()
        {
            while (!_serverDisconnectedEvent.WaitOne(100))
            {
                object message = null;
                lock (_inboundQueueLock)
                {
                    if (_inboundQueue.Count > 0)
                    {
                        message = _inboundQueue.Dequeue();
                    }
                }

                if (message != null)
                {
                    HandleMessage(message);
                }
            }
        
        }

        protected void AddOutboundMessage(object message)
        {
            lock (_inboundQueueLock)
            {
                _outboundQueue.Enqueue(message);
            }
        }

        protected void AddInboundMessage(object message)
        {
            lock (_inboundQueueLock)
            {
                _inboundQueue.Enqueue(message);
            }
        }


        protected void PumpMessages()
        {
            bool serverDown = false;

            while (!serverDown && !_serverDisconnectedEvent.WaitOne(0))
            {
                IList readList = new List<Socket>() { socket.RawSocket };

                // now let's wait for messages
                Socket.Select(readList, null, null, 100);

                object message = null;

                // there is only one socket in the poll list
                // so if the count is greater than 0 then
                // the only one available should be the client socket
                if (readList.Count > 0)
                {
                    // if socket is selected, and if available byes is 0, 
                    // then socket has been closed
                    serverDown = socket.RawSocket.Available == 0;
                    if (!serverDown)
                    {
                        byte[] objectData = socket.ReceiveData();

                        // it should be a server message, we can look
                        // at other message types later
                        message = ObjectSerialize.Deserialize(objectData);

                        AddInboundMessage(message);
                    }

                }
            }

            _serverDisconnectedEvent.Set();
        }

        private void HandleMessage(object message)
        {
            if (message.GetType() == typeof(ListUsersInRoomResponse))
            {
                OnListUsersInRoomResponseReceived(
                    new ListUsersInRoomResponseArgs()
                    {
                        Response = (ListUsersInRoomResponse)message
                    }
                );
            }
            else if (message.GetType() == typeof(GetRoomVariableResponse))
            {
                OnGetRoomVariableResponseRecieved(
                    new GetRoomVariableResponseArgs() 
                    { 
                        Response = (GetRoomVariableResponse)message 
                    }
                );
            }
            else if (message.GetType() == typeof(ServerMessage))
            {
                OnServerMessageReceived( 
                    new ServerMessageReceivedArgs() 
                    { 
                        Message = ((ServerMessage)message).Message 
                    }
                );
            }
            else if (message.GetType() == typeof(LoginResponse))
            {
                OnLoginResponseReceieved(
                    new LoginResponseEventArgs() 
                    { 
                        LoginResponse = (LoginResponse)message 
                    }
                );
            }
        }


        public void ChangeRoom(string roomname)
        {
            // change room
            SendObject(EncryptionType.AES, 
                new ChangeRoomRequest() 
                { 
                    RoomName = roomname 
                }, 
                true
            );
        }

        public void ListUsersInRoom()
        {
            SendObject(EncryptionType.AES, 
                new ListUsersInRoomRequest(), 
                true
            );
        }

        public void GetRoomVariable(string room, string varname)
        {
            SendObject(EncryptionType.AES,
                new GetRoomVariableRequest()
                {
                    RoomName = room,
                    VariableName = varname
                },
                true
            );
        }

        public void CreateRoomVariable(string room, string name, ServerObject value)
        {
            SendObject(EncryptionType.AES,
                new CreateRoomVariableRequest()
                {
                    Room = room,
                    VariableName = name,
                    Value = value
                },
                true
            );
        }
    }
}
