using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using SocketServer.Messaging;
using SocketServer.Net;
using SocketServer.Net.Client;

namespace SocketServer.Command
{
    [Serializable]
    public class ClientConnectingCommand : BaseMessageHandler
    {
        private Guid clientId;

        public ClientConnectingCommand(Guid clientId)
        {
            this.clientId = clientId;
        }

        public override void Execute()
        {
            var connection = ConnectionRepository.Instance.NewConnection();
            connection.ClientId = clientId;
        }
    }
}
