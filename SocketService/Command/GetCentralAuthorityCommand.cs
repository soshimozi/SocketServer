using System;
using SocketService.Core.Crypto;
using SocketService.Core.Messaging;
using SocketService.Net.Client;

namespace SocketService.Command
{
    [Serializable]
    public class GetCentralAuthorityCommand : BaseMessageHandler
    {
        private readonly Guid _clientId;
        public GetCentralAuthorityCommand(Guid clientId)
        {
            _clientId = clientId;
        }

        public override void Execute()
        {
            var ca = new CentralAuthority(CAKeyProtocol.DH64);

            ClientConnection connection = ConnectionRepository.Instance.FindConnectionByClientId(_clientId);
            if (connection != null)
            {
                connection.Provider = ca.GetProvider();
                MSMQQueueWrapper.QueueCommand(new SendObjectCommand(_clientId, ca));
            }
        }
    }
}
