using System;
using System.Configuration;
using System.Reflection;
using SocketService.Command;
using SocketService.Core.Configuration;
using SocketService.Core.Messaging;
using SocketService.Net.Client;
using log4net;

namespace SocketService.Net
{
    public class SocketManager //: IServerContext
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly SocketServer _socketServer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketManager"/> class.
        /// </summary>
        public SocketManager()
        {
            // create server, doesn't start pumping until we get the Start command
            _socketServer = new SocketServer();

            _socketServer.ClientConnecting += SocketServerClientConnecting;
            _socketServer.ClientDisconnecting += SocketServerClientDisconnecting;
            _socketServer.DataRecieved += SocketServerDataRecieved;
        }

        protected void SocketServerDataRecieved(object sender, DataRecievedArgs e)
        {
            ClientConnection connection = ConnectionRepository.Instance.FindConnectionByClientId(e.ClientId);
            if (connection != null)
            {
                ParseRequest(connection.ClientId, e.Data);
            }
        }

        protected void SocketServerClientConnecting(object sender, ConnectArgs e)
        {
            SocketRepository.Instance.AddSocket(e.ClientId, e.RawSocket);

            var connection = new ClientConnection(e.ClientId);
            ConnectionRepository.Instance.AddConnection(connection);
        }

        protected void SocketServerClientDisconnecting(object sender, DisconnectedArgs e)
        {
            MSMQQueueWrapper.QueueCommand(new LogoutUserCommand(e.ClientId));
        }

        /// <summary>
        /// Starts the server.
        /// </summary>
        public void StartServer()
        {
            SocketServiceConfiguration configuration = null;
            try
            {
                configuration =
                    (SocketServiceConfiguration) ConfigurationManager.GetSection("SocketServerConfiguration");
            }
            catch (Exception exception)
            {
                Logger.Error(exception.ToString());
            }

            if (configuration != null)
            {
                MSMQQueueWrapper.QueueCommand(new ServerStartingCommand());

                _socketServer.StartServer(configuration.ListenPort);
            }
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        public void StopServer()
        {
            _socketServer.StopServer();
        }

        private static void ParseRequest(Guid clientId, byte[] requestData)
        {
            MSMQQueueWrapper.QueueCommand(new ParseRequestCommand(clientId, requestData));
        }
    }
}