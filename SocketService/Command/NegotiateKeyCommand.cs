using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketServer.Messaging;
using SocketServer.Net.Client;
using SocketServer.Shared.Response;
using SocketServer.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto;

namespace SocketServer.Command
{
    [Serializable]
    public class NegotiateKeysCommand : BaseMessageHandler
    {
        private readonly Guid _clientGuid;
        public NegotiateKeysCommand(Guid clientGuid)
        {
            _clientGuid = clientGuid;
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        public override void Execute()
        {
            ClientConnection connection = ConnectionRepository.Instance.Query(c => c.ClientId == _clientGuid).FirstOrDefault();
            if (connection != null)
            {
                //connection.Provider.RemotePublicKey = new DHPublicKeyParameters(
                //    ((DHPublicKeyParameters)PublicKeyFactory.CreateKey(_remotePublicKey)).Y, connection.Provider.Parameters);

                //MSMQQueueWrapper.QueueCommand(
                //    new SendMessageCommand<NegotiateKeyResponse>(_clientGuid,
                //                          new NegotiateKeyResponse { RemotePublicKey = connection.Provider.GetEncryptedPublicKey() })
                //    );
            }
        }
    }
}
