using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using SocketService.Framework.Client.Sockets;
using SocketService.Net.Client;
using SocketService.Net;
using SocketService.Framework.Messaging;
using SocketService.Framework.Request;
using SocketService.Framework.Client.Response;

namespace SocketService.Command
{
    [Serializable()]
    public class NegotiateKeysCommand : BaseMessageHandler
    {
        Guid _clientId;
        byte[] _publicKey;

        public NegotiateKeysCommand(Guid clientId, byte[] publicKey)
        {
            _publicKey = publicKey;
            _clientId = clientId;
        }

        public override void Execute()
        {
            Connection connection = ConnectionRepository.Instance.FindConnectionByClientId(_clientId);
            if (connection != null)
            {
                //connection.RemotePublicKey = connection.Provider.Import(_data);

                //ZipSocket zipSocket = SocketRepository.Instance.FindByClientId(connection.ClientId);
                //if (zipSocket != null)
                //{
                //    zipSocket.SendData(connection.Provider.PublicKey.ToByteArray());

                //    // we are done here
                //    connection.ConnectionState = ConnectionState.Connected;
                //}

                //NegotiateKeysRequest negotiateKeysRequest = request as NegotiateKeysRequest;
                //Guid clientId = (Guid)state;

                //Connection connection = ConnectionRepository.Instance.FindConnectionByClientId(clientId);
                //if (connection != null)
                //{
                // import clients public key
                connection.RemotePublicKey = connection.Provider.Import(_publicKey);

                // send our public key back
                NegotiateKeysResponse response = new NegotiateKeysResponse();
                response.RemotePublicKey = connection.Provider.PublicKey.ToByteArray();

                // now we send a response back
                MSMQQueueWrapper.QueueCommand(new SendObjectCommand(_clientId, response));
                //}
            }
        }
    }
}
