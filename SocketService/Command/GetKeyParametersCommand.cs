using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketServer.Core.Messaging;
using SocketServer.Net.Client;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;
using SocketServer.Crypto;
using SocketServer.Shared.Response;

namespace SocketServer.Command
{
    [Serializable]
    public class GetKeyParametersCommand : BaseMessageHandler
    {
        private readonly Guid _clientId;
        public GetKeyParametersCommand(Guid clientId)
        {
            _clientId = clientId;
        }

        public override void Execute()
        {
            ClientConnection connection = ConnectionRepository.Instance.Query(c => c.ClientId == _clientId).FirstOrDefault();
            if (connection != null)
            {
                connection.Provider = new DHProvider(DHParameterHelper.GenerateParameters());

                //connection.Provider.Parameters = DHParameterHelper.GenerateParameters();

                MSMQQueueWrapper.QueueCommand(
                    new SendObjectCommand(_clientId,
                                          new GetKeyParametersResponse { G = connection.Provider.Parameters.G.ToByteArray(), P = connection.Provider.Parameters.P.ToByteArray() })
                    );

            }

        }
    }
}
