using System;
using System.IO;
using System.Linq;
using System.Configuration;
using System.Reflection;
using log4net;
using SocketServer.Command;
using SocketServer.Configuration;
using SocketServer.Net.Client;
using SocketServer.Crypto;
using System.Xml.Serialization;
using SocketServer.Shared.Header;
using SocketServer.Shared;
using System.Collections.Generic;
using SocketServer.Shared.Serialization;
using SocketServer.Shared.Reflection;
using SocketServer.Shared.Network;
using SocketServer.Repository;
using SocketServer.Messages;
using Google.ProtocolBuffers.DescriptorProtos;
using SocketServer.Shared.Messaging;
using com.BlazeServer.Messages.MessageProtos;
using Google.ProtocolBuffers;
using System.Threading;

namespace SocketServer.Net
{
    public class SocketManager //: IServerContext
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private Dictionary<string, RequestHandlerConfigurationElement> requestHandlers = new Dictionary<string, RequestHandlerConfigurationElement>();

        private readonly object handlerLock = new object();

        private readonly SocketServer _socketServer;

        //public event EventHandler<ClientRequestEventArgs> ClientRequestReceived;

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketManager"/> class.
        /// </summary>
        public SocketManager(SocketServerConfiguration config)
        {
            LoadHandlers(config);

            // TODO: Move filename into configuration
            _socketServer = new SocketServer(new MessageRegistry("messages.desc"));

            _socketServer.ClientConnected += SocketServerClientConnecting;
            _socketServer.ClientDisconnected += SocketServerClientDisconnecting;
            _socketServer.MessageReceived += new EventHandler<MessageEventArgs>(socketServer_MessageReceived);
        }

        void socketServer_MessageReceived(object sender, MessageEventArgs e)
        {
            lock (handlerLock)
            {
                if (requestHandlers.ContainsKey(e.Message.DescriptorForType.FullName))
                {
                    ServiceHandlerRepository
                    .Instance
                    .InvokeHandler(
                        e.Message.DescriptorForType.FullName, 
                        e.Message, 
                        e.ClientConnection);
                }
            }
        }

        protected void SocketServerClientConnecting(object sender, ConnectArgs e)
        {
            Logger.InfoFormat("Client {0} connecting from {1}", e.ClientId, e.RemoteAddress);

            e.Connection.ServerAuthority 
                = ServerAuthorityFactory.CreateServerAuthority();

            ServerConnectionResponse.Builder newResponse = ServerConnectionResponse.CreateBuilder();
            ServerConnectionResponse.Types.KeyParameters.Builder keyBuilder = ServerConnectionResponse.Types.KeyParameters.CreateBuilder();

            keyBuilder.SetP(e.Connection.ServerAuthority.P.ToString(16));
            keyBuilder.SetG(e.Connection.ServerAuthority.G.ToString(16));

            newResponse.SetParameters(keyBuilder.Build());
            newResponse.SetMessageId(23);
            e.Connection.Send(newResponse.Build());
        }

        protected void SocketServerClientDisconnecting(object sender, DisconnectedArgs e)
        {
            //Thread thread = new Thread(new ParameterizedThreadStart(
            //        (cmd) =>
            //        {
            //            ((ICommand)cmd).Execute();
            //        })
            //    );

            //thread.Start(new LogoutUserCommand(e.Connection));
                
            MSMQQueueWrapper.QueueCommand(new LogoutUserCommand(e.Connection));
        }

        //protected virtual void OnClientRequestReceived(Guid clientId, RequestHeader header, object request)
        //{
        //    EventHandler<ClientRequestEventArgs> clientRequestReceived = ClientRequestReceived;
        //    if (clientRequestReceived != null)
        //    {
        //        var args = new ClientRequestEventArgs(clientId, header, request);
        //        clientRequestReceived(this, args);
        //    }
        //}

        /// <summary>
        /// Starts the server.
        /// </summary>
        public void StartServer()
        {
            SocketServerConfiguration configuration = null;
            try
            {
                configuration = ServerConfigurationHelper.GetServerConfiguration();
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
                requestHandlers.Add(element.Key, element);
            }
        }
    }
}