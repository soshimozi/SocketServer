using System;
using System.Linq;
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

            ClientConnection connection =
                ConnectionRepository.Instance.Query(c => c.ClientId == _clientId).FirstOrDefault();
            if (connection == null) return;

            connection.SecureKeyProvider = ca.GetProvider();
            MSMQQueueWrapper.QueueCommand(new SendObjectCommand(_clientId, ca));
        }
    }
}
