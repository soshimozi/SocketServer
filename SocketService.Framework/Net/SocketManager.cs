using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using System.IO;
using SocketService.Framework.Messaging;
using SocketService.Framework.Net.Client;
using SocketService.Framework.Command;
using SocketService.Framework.Configuration;
using System.Configuration;

namespace SocketService.Framework.Net
{
    public class SocketManager //: IServerContext
    {
        private readonly SocketServer _socketServer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketManager"/> class.
        /// </summary>
        public SocketManager()
        {
            // create server, doesn't start pumping until we get the Start command
            _socketServer = new SocketServer();

            _socketServer.ClientConnecting += new EventHandler<ConnectArgs>(SocketServer_ClientConnecting);
            _socketServer.ClientDisconnecting += new EventHandler<DisconnectedArgs>(SocketServer_ClientDisconnecting);
            _socketServer.DataRecieved += new EventHandler<DataRecievedArgs>(SocketServer_DataRecieved);
        }

        protected void SocketServer_DataRecieved(object sender, DataRecievedArgs e)
        {
            Connection connection = ConnectionRepository.Instance.FindConnectionByClientId(e.ClientId);
            if (connection != null)
            {
                ParseRequest(connection.ClientId, e.Data);
            }
        }

        protected void SocketServer_ClientConnecting(object sender, ConnectArgs e)
        {
            SocketRepository.Instance.AddSocket(e.ClientId, e.RawSocket);

            Connection connection = new Connection(e.ClientId);
            ConnectionRepository.Instance.AddConnection(connection);
        }

        protected void SocketServer_ClientDisconnecting(object sender, DisconnectedArgs e)
        {
            MSMQQueueWrapper.QueueCommand(new LogoutUserCommand(e.ClientId));
        }

        /// <summary>
        /// Starts the server.
        /// </summary>
        /// <param name="port">The port.</param>
        public void StartServer()
        {
            SocketServiceConfiguration configuration = null;
            try
            {
                configuration = (SocketServiceConfiguration)ConfigurationManager.GetSection("SocketServerConfiguration");
            }
            catch (Exception ex)
            { }

            if (configuration != null)
            {
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

        private void ParseRequest(Guid clientId, byte[] requestData)
        {
            MSMQQueueWrapper.QueueCommand(new ParseRequestCommand(clientId, requestData));
        }
    }
}
