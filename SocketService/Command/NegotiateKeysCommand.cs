using System;
using System.Linq;
using SocketService.Core.Messaging;
using SocketService.Net.Client;
using SocketService.Shared.Response;

namespace SocketService.Command
{
    [Serializable]
    public class NegotiateKeysCommand : BaseMessageHandler
    {
        private readonly Guid _clientId;
        private readonly byte[] _publicKey;

        public NegotiateKeysCommand(Guid clientId, byte[] publicKey)
        {
            _publicKey = publicKey;
            _clientId = clientId;
        }

        public override void Execute()
        {
            var connection =
                ConnectionRepository.Instance.Query(c => c.ClientId == _clientId).FirstOrDefault();

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
                connection.RemotePublicKey = connection.SecureKeyProvider.Import(_publicKey);

                // send our public key back
                var response = new NegotiateKeysResponse {RemotePublicKey = connection.SecureKeyProvider.PublicKey.ToByteArray()};

                // now we send a response back
                MSMQQueueWrapper.QueueCommand(new SendObjectCommand(_clientId, response));
                //}
            }
        }
    }
}