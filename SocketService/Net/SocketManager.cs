using System;
using System.IO;
using System.Linq;
using System.Configuration;
using System.Reflection;
using log4net;
using SocketServer.Command;
using SocketServer.Configuration;
using SocketServer.Messaging;
using SocketServer.Net.Client;
using SocketServer.Crypto;
using SocketServer.Shared.Interop.Java;
using System.Xml.Serialization;
using SocketServer.Shared.Header;
using SocketServer.Shared;
using System.Collections.Generic;
using SocketServer.Shared.Serialization;
using SocketServer.Reflection;

namespace SocketServer.Net
{
    public class SocketManager //: IServerContext
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private Dictionary<string, RequestHandlerConfigurationElement> _configuration = new Dictionary<string, RequestHandlerConfigurationElement>();

        private readonly SocketServer _socketServer;

        public event EventHandler<ClientRequestEventArgs> ClientRequestReceived;

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketManager"/> class.
        /// </summary>
        public SocketManager(SocketServerConfiguration config)
        {
            LoadHandlers(config);

            // create server, doesn't start pumping until we get the Start command
            _socketServer = new SocketServer();

            _socketServer.ClientConnecting += SocketServerClientConnecting;
            _socketServer.ClientDisconnecting += SocketServerClientDisconnecting;
            _socketServer.DataRecieved += SocketServerDataRecieved;
        }

        protected void SocketServerDataRecieved(object sender, DataRecievedArgs e)
        {
            var connection = ConnectionRepository.Instance.Query( c => c.ClientId == e.ClientId).FirstOrDefault();
            if (connection != null)
            {
                // update client buffer with new data
                connection.ClientBuffer.Write(e.Data);

                bool done = false;
                while (!done)
                {
                    done = true;

                    MemoryStream stream = new MemoryStream(connection.ClientBuffer.Buffer);
                    switch (connection.CurrentState)
                    {
                        case ProtocolState.WaitingForHeader:
                            {
                                // check if there is a utf string available for reading
                                // low level protocol is all utf strings
                                string utfString = stream.ReadUTF();
                                if (utfString != null)
                                {
                                    //TextReader reader = new StringReader(utfString);
                                    connection.RequestHeader 
                                        = XmlSerializationHelper
                                            .DeSerialize<RequestHeader>(utfString);

                                    // if we got a valid request header, move to next state
                                    // otherwise wait until we get a valid request header
                                    if (connection.RequestHeader != null)
                                    {
                                        connection.CurrentState = ProtocolState.WaitingForBody;
                                    }
                                }
                            }
                            break;

                        case ProtocolState.WaitingForBody:
                            {
                                string rawRequestString = stream.ReadUTF();
                                if (rawRequestString != null)
                                {
                                    if (_configuration.ContainsKey(connection.RequestHeader.RequestType.ToString()))
                                    {
                                        string key = connection.RequestHeader.RequestType.ToString();

                                        string requestHandlerType = _configuration[key].HandlerType;
                                        string requestType = _configuration[key].RequestType;
                                        string requestString = rawRequestString;

                                        MSMQQueueWrapper.QueueCommand(
                                            new HandleClientRequestCommand(
                                                connection.ClientId,
                                                key,
                                                requestType, 
                                                requestString));
                                    }

                                    connection.CurrentState = ProtocolState.WaitingForHeader;
                                }
                            }
                            break;
                    }

                    // fix up buffers
                    connection.ClientBuffer = new ClientBuffer();

                    // if any data left, fix up buffers
                    if (stream.Length > stream.Position)
                    {
                        // left over bytes
                        byte[] leftover = stream.Read((int)(stream.Length - stream.Position));
                        connection.ClientBuffer.Write(leftover);

                        // we are not done reading
                        done = false;
                    }
                }
            }
        }



        protected void SocketServerClientConnecting(object sender, ConnectArgs e)
        {
            Logger.InfoFormat("Client {0} connecting from {1}", e.ClientId, e.RemoteAddress);
            SocketRepository.Instance.AddSocket(e.ClientId, e.RawSocket);

            var connection = ConnectionRepository.Instance.NewConnection();
            connection.ClientId = e.ClientId;
        }

        protected void SocketServerClientDisconnecting(object sender, DisconnectedArgs e)
        {
            MSMQQueueWrapper.QueueCommand(new LogoutUserCommand(e.ClientId));
        }

        protected virtual void OnClientRequestReceived(Guid clientId, RequestHeader header, object request)
        {
            EventHandler<ClientRequestEventArgs> clientRequestReceived = ClientRequestReceived;
            if (clientRequestReceived != null)
            {
                var args = new ClientRequestEventArgs(clientId, header, request);
                clientRequestReceived(this, args);
            }
        }
        /// <summary>
        /// Starts the server.
        /// </summary>
        public void StartServer()
        {
            SocketServerConfiguration configuration = null;
            try
            {
                configuration =
                    (SocketServerConfiguration) ConfigurationManager.GetSection("SocketServerConfiguration");
            }
            catch (Exception exception)
            {
                Logger.Error(exception.ToString());
            }

            if (configuration != null)
            {
                MSMQQueueWrapper.QueueCommand(new ServerStartingCommand());

                _socketServer.StartServer(configuration.ListenPort);

                Logger.InfoFormat("Server started and listening on {0}", configuration.ListenPort);
            }
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        public void StopServer()
        {
            _socketServer.StopServer();
        }

        //private static void ParseRequest(Guid clientId, byte[] requestData)
        //{
        //    MSMQQueueWrapper.QueueCommand(new ParseRequestCommand(clientId, requestData));
        //}


        private void LoadHandlers(SocketServerConfiguration config)
        {
            foreach (RequestHandlerConfigurationElement element in config.Handlers)
            {
                // try to find enum type first
                _configuration.Add(element.Key, element);
            }
        }
    }
}