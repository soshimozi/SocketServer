using System;
using System.Collections.Generic;
using System.Threading;
using System.Collections;
using System.Net.Sockets;
using SocketService.Crypto;
using SocketService.Framework.Client.Request;
using SocketService.Framework.Client.Sockets;
using SocketService.Framework.Client.Serialize;
using SocketService.Framework.Client.Response;
using SocketService.Client.API.Event;
using SocketService.Framework.Client.Event;

namespace SocketService.Client.API
{
    public class Server
    {
        private readonly ManualResetEvent _serverDisconnectedEvent = new ManualResetEvent(false);
        private DiffieHellmanKey _remotePublicKey;
        private DiffieHellmanProvider _provider;
        private ZipSocket _socket;
        private bool _connected;

        public event EventHandler<ConnectionEventArgs> ConnectionResponse;
        public event EventHandler<ServerResponseEventArgs> ServerResponse;
        public event EventHandler<ServerEventEventArgs> ServerEvent;

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        public void Disconnect()
        {
            _serverDisconnectedEvent.Set();
            _connected = false;
        }

        /// <summary>
        /// Connects the specified address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="port">The port.</param>
        public void Connect(string address, int port)
        {
            if (!_connected)
            {
                var rawSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _serverDisconnectedEvent.Reset();

                bool connected = false;
                while (!connected)
                {
                    try
                    {
                        rawSocket.Connect(address, port);
                        connected = true;
                    }
                    catch (SocketException)
                    {
                        OnConnectionResponse(new ConnectionEventArgs { IsSuccessful = false });
                        return;
                    }
                }

                // wrap the socket
                _socket = new ZipSocket(rawSocket);
                NegotiateKeys();

                OnConnectionResponse(new ConnectionEventArgs { IsSuccessful = true });
                _connected = true;

                var serverThread = new Thread(PumpMessages);
                serverThread.Start();
            }

        }

        protected virtual void OnConnectionResponse(ConnectionEventArgs args)
        {
            var func = ConnectionResponse;
            if (func != null)
            {
                func(this, args);
            }

        }

        protected void PumpMessages()
        {
            var serverDown = false;

            while (!serverDown && !_serverDisconnectedEvent.WaitOne(0))
            {
                IList readList = new List<Socket> { _socket.RawSocket };

                // now let's wait for messages
                Socket.Select(readList, null, null, 100);

                // there is only one socket in the poll list
                // so if the count is greater than 0 then
                // the only one available should be the client socket
                if (readList.Count > 0)
                {
                    // if socket is selected, and if available byes is 0, 
                    // then socket has been closed
                    serverDown = _socket.RawSocket.Available == 0;
                    if (!serverDown)
                    {
                        var objectData = _socket.ReceiveData();

                        // it should be a server message, we can look
                        // at other message types later
                        var message = ObjectSerialize.Deserialize(objectData);

                        DispatchMessage(message);
                    }

                }
            }

            _serverDisconnectedEvent.Set();
        }

        private void DispatchMessage(object message)
        {
            if (message is IResponse)
                OnServerResponse(new ServerResponseEventArgs {Response = message as IResponse});
            else if (message is IEvent)
                OnServerEvent(new ServerEventEventArgs {ServerEvent = message as IEvent});
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
            IList readList = new List<Socket> { _socket.RawSocket };
            while (!doneHandshaking)
            {
                Socket.Select(readList, null, null, -1);

                // there is only one socket in the poll list
                // so if the count is greater than 0 then
                // the only one available should be the client socket
                if (readList.Count > 0)
                {
                    var availableBytes = _socket.RawSocket.Available;
                    if (availableBytes > 0)
                    {
                        var objectData = _socket.ReceiveData();
                        switch (step)
                        {
                            case 0:
                                var ca = ObjectSerialize.Deserialize<CentralAuthority>(objectData);
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
                                var response = ObjectSerialize.Deserialize<NegotiateKeysResponse>(objectData);
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

        /// <summary>
        /// Sends the request.
        /// </summary>
        /// <param name="requestData">The request data.</param>
        public void SendRequest(object requestData)
        {
            _socket.SendData(CreateRequest(requestData, false));
        }

        /// <summary>
        /// Sends the request encrypted.
        /// </summary>
        /// <param name="requestData">The request data.</param>
        public void SendRequestEncrypted(object requestData)
        {
            _socket.SendData(CreateRequest(requestData, true));
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
                        new ClientRequestWrapper(cryptoWrapper.IV,
                           EncryptionType.TripleDES,
                           DateTime.Now, 0,
                           cryptoWrapper.Encrypt(ObjectSerialize.Serialize(requestData))
                           )
                        );
                }
            }
            
            return ObjectSerialize.Serialize(
                new ClientRequestWrapper(new byte[] { },
                                         EncryptionType.None,
                                         DateTime.Now, 0,
                                         ObjectSerialize.Serialize(requestData)
                    )
                );
        }

    }
}
