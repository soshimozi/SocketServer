using System;
using System.Linq;
using System.Configuration;
using System.Reflection;
using SocketServer.Command;
using SocketServer.Core.Configuration;
using SocketServer.Core.Messaging;
using SocketServer.Net.Client;
using log4net;
using SocketServer.Crypto;

namespace SocketServer.Net
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
            var connection = ConnectionRepository.Instance.Query( c => c.ClientId == e.ClientId).FirstOrDefault();
            if (connection != null)
            {
                ParseRequest(connection.ClientId, e.Data);
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

        private static void ParseRequest(Guid clientId, byte[] requestData)
        {
            MSMQQueueWrapper.QueueCommand(new ParseRequestCommand(clientId, requestData));
        }
    }
}