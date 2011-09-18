using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using SocketService.Framework.Client.Sockets;
using SocketService.Net.Client;
using SocketService.Net;
using SocketService.Framework.Messaging;

namespace SocketService.Command
{
    [Serializable()]
    public class NegotiateKeysCommand : ICommand
    {
        Guid _clientId;
        byte[] _data;

        public NegotiateKeysCommand(Guid clientId, byte[] publicKeyData)
        {
            _data = publicKeyData;
            _clientId = clientId;
        }

        public void Execute()
        {
            Connection connection = ConnectionRepository.Instance.FindConnectionByClientId(_clientId);
            if (connection != null)
            {
                connection.RemotePublicKey = connection.Provider.Import(_data);

                ZipSocket zipSocket = SocketRepository.Instance.FindByClientId(connection.ClientId);
                if (zipSocket != null)
                {
                    zipSocket.SendData(connection.Provider.PublicKey.ToByteArray());

                    // we are done here
                    connection.ConnectionState = ConnectionState.Connected;
                }
            }
        }
    }
}
