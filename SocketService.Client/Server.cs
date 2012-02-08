using System;
using System.Collections.Generic;
using System.Threading;
using System.Collections;
using System.Net.Sockets;
using SocketServer.Crypto;
using SocketServer.Client;
using SocketServer.Shared;
using SocketServer.Shared.Request;
using SocketServer.Shared.Response;
using SocketServer.Shared.Sockets;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using SocketServer.Shared.Header;
using SocketServer.Shared.Serialization;
using SocketServer.Shared.Interop.Java;
using System.IO;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.X509;

namespace SocketServer.Client
{
    public class Server
    {
        private readonly ManualResetEvent _serverDisconnectedEvent = new ManualResetEvent(false);

        private ZipSocket _socket = null;
        //private DHProvider _provider = new DHProvider(SocketServer.Crypto.Constants.DefaultDiffieHellmanKeyLength, SocketServer.Crypto.Constants.DefaultPrimeProbability);

        private bool _connected;

        public event EventHandler<ConnectionEventArgs> ConnectionResponse;
        public event EventHandler<ServerResponseEventArgs> ServerResponse;
        public event EventHandler<ServerEventEventArgs> ServerEvent;
        public event EventHandler<CommunicationErrorArgs> CommunicationError;

        private ServerAuthority _serverAuthority = null;
        private byte[] _serverPublicKey = null;

        private ClientBuffer buffer = new ClientBuffer();

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

                if (!NegotiateKeys())
                {
                    // we could not negotiate keys, so we can't connect
                    OnConnectionResponse(new ConnectionEventArgs { IsSuccessful = false });
                }
                else
                {
                    OnConnectionResponse(new ConnectionEventArgs { IsSuccessful = true });
                    _connected = true;

                    // start pumping messages
                    var serverThread = new Thread(PumpMessages);
                    serverThread.Start();
                }
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

            int step = 0;
            ResponseHeader header = null;
            while (!serverDown && !_serverDisconnectedEvent.WaitOne(0))
            {
                // wait on header

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
                        byte[] data = _socket.ReceiveData();
                        buffer.Write(data);

                        MemoryStream stream = new MemoryStream(buffer.Buffer);
                        switch (step)
                        {
                            case 0: /* reading header */
                                {
                                    string utfString = stream.ReadUTF();
                                    if (utfString != null)
                                    {
                                        //TextReader reader = new StringReader(utfString);
                                        header = XmlSerializationHelper
                                                .DeSerialize<ResponseHeader>(utfString);

                                        step++;
                                    }
                                }
                                break;

                            case 1:
                                {
                                    string rawRequestString = stream.ReadUTF();
                                    if (rawRequestString != null && header != null)
                                    {
                                        ProcessResponse(header, rawRequestString);
                                        step = 0;
                                    }
                                }
                                break;
                        }

                        // fix up buffers
                        buffer = new ClientBuffer();

                        // if any data left, fix up buffers
                        if (stream.Length > stream.Position)
                        {
                            // left over bytes
                            byte[] leftover = stream.Read((int)(stream.Length - stream.Position));
                            buffer.Write(leftover);
                        }

                    }

                    var objectData = _socket.ReceiveData();

                    // it should be a server message, we can look
                    // at other message types later
                    //var message = ObjectSerialize.Deserialize(objectData);

                    //DispatchMessage(message);
                }

            }
            

            _serverDisconnectedEvent.Set();
        }

        private void ProcessResponse(ResponseHeader header, string rawRequestString)
        {
            if (header.MessageHeader.EncryptionHeader.EncryptionType != EncryptionTypes.None)
            {
                DHPublicKeyParameters publicKey = new DHPublicKeyParameters(
                    ((DHPublicKeyParameters)PublicKeyFactory.CreateKey(_serverPublicKey)).Y, _serverAuthority.Parameters);

                BigInteger agreementValue = _serverAuthority.GenerateAgreementValue(publicKey);

                RijndaelCrypto crypto = new RijndaelCrypto();
                rawRequestString = crypto.Decrypt(rawRequestString, agreementValue.ToString(16));

                //rawRequestString = ResponseBuilder.ProcessResponse(_serverAuthority, _serverPublicKey, header, rawRequestString);
            }

            //switch(header.ResponseType)
            //{
            //    case ResponseTypes.LoginResponse:
            //        {
            //        LoginResponse response = XmlSerializationHelper.DeSerialize<LoginResponse>(rawRequestString);
            //            OnServerEvent(new ServerEventEventArgs() { ServerEvent = new LoginResponseEvent  })
            //}
            //val = XmlSerializationHelper.DeSerialize<T>(
            //    ResponseBuilder
            //    .ProcessResponse(
            //            _serverAuthority,
            //            _serverPublicKey,
            //            header,
            //            rawRequestString));
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

        private bool NegotiateKeys()
        {
            SendRequest<NegotiateKeysRequest>(RequestTypes.NegotiateKeysRequest, new NegotiateKeysRequest(), false);

            // wait 10 seconds for response
            NegotiateKeysResponse response = WaitForResponse<NegotiateKeysResponse>();
            if (response == null)
            {
                return false;
            }

            // we have a response, let's copy the server key and create the serverauthority
            _serverAuthority = new ServerAuthority(new BigInteger(response.Prime, 16), new BigInteger(response.G, 16));
            _serverPublicKey = response.ServerPublicKey;

            return true;
            // now wait for response

            //NetworkStream stream = new NetworkStream(_socket.RawSocket);

             //_socket .SendData(CreateRequest(new NegotiateKeysRequest(), false));

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


        private T WaitForResponse<T>() where T : class
        {
            T val = default(T);

            ResponseHeader header = null;
            int step = 0;

            while (step < 2)
            {
                byte [] data = _socket.ReceiveData();
                buffer.Write(data);

                MemoryStream stream = new MemoryStream(buffer.Buffer);
                switch (step)
                {
                    case 0: /* reading header */
                        {
                            string utfString = stream.ReadUTF();
                            if (utfString != null)
                            {
                                //TextReader reader = new StringReader(utfString);
                                header = XmlSerializationHelper
                                        .DeSerialize<ResponseHeader>(utfString);

                                step++;

                                // if we got a valid request header, move to next state
                                // otherwise wait until we get a valid request header
                                //if (header != null)
                                //{
                                //    step++;
                                //}
                            }
                        }
                        break;

                    case 1:
                        {
                            string rawRequestString = stream.ReadUTF();
                            if (rawRequestString != null)
                            {
                                if (header != null)
                                {
                                    val = XmlSerializationHelper.DeSerialize<T>(
                                        ResponseBuilder
                                        .ProcessResponse(
                                                _serverAuthority,
                                                _serverPublicKey,
                                                header,
                                                rawRequestString));
                                }

                                step++;
                            }
                        }
                        break;
                }

                // fix up buffers
                buffer = new ClientBuffer();

                // if any data left, fix up buffers
                if (stream.Length > stream.Position)
                {
                    // left over bytes
                    byte[] leftover = stream.Read((int)(stream.Length - stream.Position));
                    buffer.Write(leftover);
                }

            }

            return val;

        }

        //private T WaitForResponse<T>() where T : class
        //{
        //    NetworkStream stream = new NetworkStream(_socket.RawSocket);
        //    return XmlSerializationHelper.DeSerialize<T>(stream.ReadUTF()) as T;
        //}

        ///// <summary>
        ///// Sends the request.
        ///// </summary>
        ///// <param name="requestData">The request data.</param>
        //private void SendRequestRaw(object requestData)
        //{
        //    _socket.SendData(CreateRequest(requestData, false));
        //}

        /// <summary>
        /// Sends the request encrypted.
        /// </summary>
        /// <param name="requestData">The request data.</param>
        public void SendRequest<T>(RequestTypes requestType, T request, bool encrypted) where T : class
        {
            string requestString = XmlSerializationHelper.Serialize<T>(request);

            RequestHeader header = null;

            byte[] publicKey = null;

            if( encrypted )
            {
                header = RequestBuilder.BuildRequestHeader(
                    EncryptionTypes.Aes, 
                    CompressionTypes.None, 
                    requestType);

                publicKey = EncryptionHelper.EncodeKeyParameter(
                    _serverAuthority.GetPublicKeyParameter());

                header.MessageHeader.EncryptionHeader.PublicKey = publicKey;

            } else
            {
                header = RequestBuilder.BuildRequestHeader(
                    EncryptionTypes.None,
                    CompressionTypes.None,
                    requestType);
            }

            _socket.SendData(XmlSerializationHelper.Serialize<RequestHeader>(header));
            _socket.SendData(RequestBuilder.ProcessRequest(
                                _serverAuthority,
                                _serverPublicKey, 
                                header,
                                XmlSerializationHelper.Serialize<T>(request)));

            //string request = RequestBuilder.ProcessRequest()

        }


        //private byte[] CreateRequest(object requestData, bool encrypt)
        //{

        //    //NegotiateKeysRequest negotiateKeysRequest = new NegotiateKeysRequest();
        //    //

        //    EncryptionHeader encryptionHeader = new EncryptionHeader();

        //    MessageHeader messageHeader = new MessageHeader();
        //    RequestHeader requestHeader = new RequestHeader();

        //    //SendRequest(negotiateKeysRequest, false)

        //    if (encrypt)
        //    {

        //        //header = new RequestHeader()
        //        //{
        //        //    MessageHeader = new MessageHeader() { CompressionType = CompressionTypes.None, EncryptionHeader = new EncryptionHeader() { EncryptionType= EncryptionTypes.Aes, PublicKey =  } }
        //        //};

        //        //using (CryptoManager cryptoWrapper =
        //        //    CryptoManager.CreateEncryptor(AlgorithmType.TripleDES,
        //        //            _provider.Agree()))
        //        //{
        //        //    return ObjectSerialize.Serialize(
        //        //        new ClientRequest(cryptoWrapper.IV,
        //        //           EncryptionType.TripleDES,
        //        //           DateTime.Now, 0,
        //        //           cryptoWrapper.Encrypt(ObjectSerialize.Serialize(requestData))
        //        //           )
        //        //        );
        //        //}
        //    }

        //    //return ObjectSerialize.Serialize(
        //    //    new ClientRequest(new byte[] { },
        //    //                             EncryptionType.None,
        //    //                             DateTime.Now, 0,
        //    //                             ObjectSerialize.Serialize(requestData)
        //    //        )
        //    //    );

        //    return null;
        //}

    }
}
