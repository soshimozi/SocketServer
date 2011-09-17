using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Crypto;
using SocketService.Serial;
using SocketService.Net.Sockets;
using System.Net.Sockets;
using SocketService.Client;
using SocketService.Net;

namespace SocketService.Command
{
    [Serializable()]
    public class ClientConnectCommand : BaseCommand
    {
        private readonly Guid _clientId;
        public ClientConnectCommand(Guid clientId)
        {
            _clientId = clientId;
        }

        public override void Execute()
        {
            ZipSocket rawSocket = SocketRepository.Instance.FindByClientId(_clientId);

            if (rawSocket != null)
            {
                Connection connection = ConnectionRepository.Instance.FindConnectionByClientId(_clientId);
                if (connection != null)
                {
                    connection.ConnectionState = ConnectionState.NegotiateKeyPair;
                }
            }
        }
    }
}
