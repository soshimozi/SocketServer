using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Client.API.Manager;
using System.Net;
using System.Threading;
using System.Collections;
using System.Net.Sockets;
using SocketService.Crypto;
using SocketService.Framework.Client.Sockets;
using SocketService.Framework.Request;
using SocketService.Framework.Client.Serialize;
using SocketService.Framework.Client.Response;
using SocketService.Client.API.Event;
using SocketService.Framework.Client.Event;

namespace SocketService.Client.API
{
    public class Server
    {
        private ManualResetEvent _serverDisconnectedEvent = new ManualResetEvent(false);
        private DiffieHellmanKey _remotePublicKey;
        private DiffieHellmanProvider _provider;
        private ZipSocket socket;
        private bool _connected = false;

        public event EventHandler<ConnectionEventArgs> ConnectionResponse;
        public event EventHandler<ServerResponseEventArgs> ServerResponse;
        public event EventHandler<ServerEventEventArgs> ServerEvent;

        public Server()
        {
        }

        public void Disconnect()
        {
            _serverDisconnectedEvent.Set();
            _connected = false;
        }

        public void Connect(string address, int port)
        {
            if (!_connected)
            {
                Socket rawSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _serverDisconnectedEvent.Reset();

                bool connected = false;
                while (!connected)
                {
                    try
                    {
                        rawSocket.Connect(address, port);
                        connected = true;
                    }
                    catch (SocketException ex)
                    {
                        OnConnectionResponse(new ConnectionEventArgs() { IsSuccessful = false });
                        return;
                    }
                }

                // wrap the socket
                socket = new ZipSocket(rawSocket, Guid.NewGuid());
                NegotiateKeys();

                OnConnectionResponse(new ConnectionEventArgs() { IsSuccessful = true });
                _connected = true;

                Thread serverThread = new Thread(new ThreadStart(PumpMessages));
                serverThread.Start();
            }

        }
    
        //public void Connect(IPEndPoint endpoint)
        //{
        //    Connect(endpoint.Address, endpoint.Port);
        //}

        //public void Connect(string address, int port)
        //{
        //    IPHostEntry hostEntry = Dns.GetHostEntry(address);
        //    if (hostEntry != null && hostEntry.AddressList.Length > 0)
        //    {
        //        IPAddress addr = hostEntry.AddressList[0];
        //        Connect(addr, port);
        //    }
        //}

        protected virtual void OnConnectionResponse(ConnectionEventArgs args)
        {
            var func = ConnectionResponse;
            if (func != null)
            {
                func(this, args);
            }

        }

        //public ClientEngine Engine
        //{
        //    get;
        //    private set;
        //}

        //public ManagerHelper Managers
        //{
        //    get;
        //    private set;
        //}

        //protected virtual void ServerThread()
        //{
        //    //// TODO: read info from configuration
        //    //Socket rawSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //    //bool connected = false;
        //    //while (!connected)
        //    //{
        //    //    try
        //    //    {
        //    //        rawSocket.Connect("127.0.0.1", 4000);
        //    //        connected = true;
        //    //    }
        //    //    catch (SocketException)
        //    //    {
        //    //        OnConnectionResponse(new ConnectionResponseEventArgs() { IsSuccessful = false });
        //    //        return;
        //    //    }
        //    //}

        //    //// wrap the socket
        //    //socket = new ZipSocket(rawSocket, Guid.NewGuid());
        //    //NegotiateKeys();

        //    //OnConnectionResponse(new ConnectionResponseEventArgs() { IsSuccessful = true });
        //    //_connected = true;

        //    ////Thread messageThread = new Thread(new ThreadStart(PumpMessages));
        //    ////messageThread.Start();
        //    //PumpMessages();
        //}

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

                        DispatchMessage(message);
                    }

                }
            }

            _serverDisconnectedEvent.Set();
        }

        private void DispatchMessage(object message)
        {
            if (message is IResponse)
            {
                OnServerResponse(new ServerResponseEventArgs() { Response = message as IResponse });
            }
            else if (message is IEvent)
            {
                OnServerEvent(new ServerEventEventArgs() { ServerEvent = message as IEvent });
            }
            else
            {
                // TODO: Handle unknown message types - this might be hacking attempts
            }

        }

        protected void OnServerEvent(ServerEventEventArgs args)
        {
            var func = ServerEvent;
            if (func != null)
            {
                func(this, args);
            }
        }

        protected void OnServerResponse(ServerResponseEventArgs args)
        {
            var func = ServerResponse;
            if (func != null)
            {
                func(this, args);
            }
        }

        private void NegotiateKeys()
        {
            SendRequest(new GetCentralAuthorityRequest());

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
                                    SendRequest(new NegotiateKeysRequest(_provider.PublicKey.ToByteArray()));

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

        public void SendRequest(object requestData)
        {
            socket.SendData(CreateRequest(requestData, false));
        }

        public void SendRequestEncrypted(object requestData)
        {
            socket.SendData(CreateRequest(requestData, true));
        }


        private byte[] CreateRequest(object requestData, bool encrypt)
        {
            if (encrypt)
            {
                using (Wrapper cryptoWrapper =
                    Wrapper.CreateEncryptor(AlgorithmType.TripleDES,
                            _provider.CreatePrivateKey(_remotePublicKey).ToByteArray()))
                {
                    return ObjectSerialize.Serialize(
                        new ClientRequest(cryptoWrapper.IV,
                           EncryptionType.TripleDES,
                           DateTime.Now, 0,
                           cryptoWrapper.Encrypt(ObjectSerialize.Serialize(requestData))
                           )
                        );
                }
            }
            else
            {
                return ObjectSerialize.Serialize(
                    new ClientRequest(new byte[0] { },
                        EncryptionType.None,
                        DateTime.Now, 0,
                        ObjectSerialize.Serialize(requestData)
                        )
                    );
            }
        }
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


    }
}
