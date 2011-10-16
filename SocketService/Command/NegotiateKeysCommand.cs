using System;
using SocketService.Framework.Client.Response;
using SocketService.Framework.Messaging;
using SocketService.Net.Client;

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
                var response = new NegotiateKeysResponse {RemotePublicKey = connection.Provider.PublicKey.ToByteArray()};

                // now we send a response back
                MSMQQueueWrapper.QueueCommand(new SendObjectCommand(_clientId, response));
                //}
            }
        }
    }
}