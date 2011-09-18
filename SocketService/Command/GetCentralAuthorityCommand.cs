using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.Messaging;
using SocketService.Framework.Crypto;
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
            CentralAuthority ca = new CentralAuthority(CAKeyProtocol.DH64);

            Connection connection = ConnectionRepository.Instance.FindConnectionByClientId(_clientId);
            if (connection != null)
            {
                connection.Provider = ca.GetProvider();
                MSMQQueueWrapper.QueueCommand(new SendObjectCommand(_clientId, ca));
            }
        }
    }
}
