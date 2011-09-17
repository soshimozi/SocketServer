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

namespace SocketService.Framework.Net
{
    public class SocketManager //: IServerContext
    {
        private readonly SocketServer _socketServer;

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

                //switch (connection.ConnectionState)
                //{
                //    case ConnectionState.Connected:
                //        ParseRequest(connection.ClientId, e.Data);
                //        break;

                //    // TODO : Move into application logic, there should only be one state : Connected or Not
                //    case ConnectionState.NegotiateKeyPair:
                //        NegotiateKeys(connection, e.Data);
                //        break;
                //}
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
            //Connection connection = ConnectionRepository.Instance.FindConnectionByClientId(e.ClientId);
            //if (connection != null)
            //{
            //    ConnectionRepository.Instance.RemoveConnection(connection);
            //}
        }

        public void StartServer(int port)
        {
            _socketServer.StartServer(port);
        }

        public void StopServer()
        {
            _socketServer.StopServer();
        }

        //private void NegotiateKeys(Connection connection, byte[] remotePublicKey)
        //{
        //    MSMQQueueCommand.QueueCommand(new NegotiateKeysCommand(connection.ClientId, remotePublicKey));
        //}

        private void ParseRequest(Guid clientId, byte[] requestData)
        {
            MSMQQueueWrapper.QueueCommand(new ParseRequestCommand(clientId, requestData));
        }
    }
}
