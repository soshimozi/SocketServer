using System;
using System.Collections.Generic;
using System.Threading;
using System.Collections;
using System.Net.Sockets;
using SocketServer.Crypto;
using SocketServer.Event;
using SocketServer.Shared;
using SocketServer.Shared.Request;
using SocketServer.Shared.Response;
using SocketServer.Shared.Sockets;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;

namespace SocketServer.Client
{
    public class Server
    {
        private readonly ManualResetEvent _serverDisconnectedEvent = new ManualResetEvent(false);

        private ZipSocket _socket = null;
        private DHProvider _provider = new DHProvider(SocketServer.Crypto.Constants.DefaultDiffieHellmanKeyLength, SocketServer.Crypto.Constants.DefaultPrimeProbability);

        private bool _connected;

        public event EventHandler<ConnectionEventArgs> ConnectionResponse;
        //public event EventHandler<ServerResponseEventArgs> ServerResponse;
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
                    catch (SocketException se)
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
                        //var message = ObjectSerialize.Deserialize(objectData);

                        //DispatchMessage(message);
                    }

                }
            }

            _serverDisconnectedEvent.Set();
        }

        //private void DispatchMessage(object message)
        //{
        //    if (message is IServerResponse)
        //        OnServerResponse(new ServerResponseEventArgs { Response = message as IServerResponse });
        //    else if (message is IEvent)
        //        OnServerEvent(new ServerEventEventArgs {ServerEvent = message as IEvent});
        //}

        protected void OnServerEvent(ServerEventEventArgs args)
        {
            var func = ServerEvent;
            if (func != null)
            {
                func(this, args);
            }
        }

        //protected void OnServerResponse(ServerResponseEventArgs args)
        //{
        //    var func = ServerResponse;
        //    if (func != null)
        //    {
        //        func(this, args);
        //    }
        //}

        private T ReadObject<T>(int timeout) where T : class
        {
            // wait for central authority
            IList readList = new List<Socket> { _socket.RawSocket };

            Socket.Select(readList, null, null, timeout);

            T response = default(T);
            if (readList.Count > 0)
            {
                var availableBytes = _socket.RawSocket.Available;
                if (availableBytes > 0)
                {
                    var objectData = _socket.ReceiveData();
                    //response = ObjectSerialize.Deserialize<T>(objectData);
                }
            }

            return response;
        }

        private void NegotiateKeys()
        {
            //SendRequestRaw(new GetKeyParametersRequest());
            //var parametersResponse = ReadObject<GetKeyParametersResponse>(-1);

            //// generate some parameters from the response (P/G values)
            //DHParameters parameters = DHParameterHelper.GenerateParameters(parametersResponse.P, parametersResponse.G);
            //_provider = new DHProvider(parameters);

            //SendRequestRaw(new NegotiateKeyRequest() { RemotePublicKey = _provider.GetEncryptedPublicKey() } );
            //var negotiateKeyResponse = ReadObject<NegotiateKeyResponse>(-1);

            //_provider.RemotePublicKey = new DHPublicKeyParameters(
            //    ((DHPublicKeyParameters)PublicKeyFactory.CreateKey(negotiateKeyResponse.RemotePublicKey)).Y, _provider.Parameters);

            //byte [] agree = _provider.Agree();
        }

        /// <summary>
        /// Sends the request.
        /// </summary>
        /// <param name="requestData">The request data.</param>
        private void SendRequestRaw(object requestData)
        {
            _socket.SendData(CreateRequest(requestData, false));
        }

        /// <summary>
        /// Sends the request encrypted.
        /// </summary>
        /// <param name="requestData">The request data.</param>
        public void SendRequest(object requestData)
        {
            _socket.SendData(CreateRequest(requestData, true));
        }


        private byte[] CreateRequest(object requestData, bool encrypt)
        {
            if (encrypt)
            {
                //using (CryptoManager cryptoWrapper =
                //    CryptoManager.CreateEncryptor(AlgorithmType.TripleDES,
                //            _provider.Agree()))
                //{
                //    return ObjectSerialize.Serialize(
                //        new ClientRequest(cryptoWrapper.IV,
                //           EncryptionType.TripleDES,
                //           DateTime.Now, 0,
                //           cryptoWrapper.Encrypt(ObjectSerialize.Serialize(requestData))
                //           )
                //        );
                //}
            }
            
            //return ObjectSerialize.Serialize(
            //    new ClientRequest(new byte[] { },
            //                             EncryptionType.None,
            //                             DateTime.Now, 0,
            //                             ObjectSerialize.Serialize(requestData)
            //        )
            //    );

            return null;
        }

    }
}
