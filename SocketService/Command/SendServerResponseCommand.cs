using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketServer.Messaging;
using SocketServer.Shared;
using SocketServer.Shared.Header;
using SocketServer.Net.Client;
using SocketServer.Crypto;
using SocketServer.Shared.Serialization;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace SocketServer.Command
{
    [Serializable]
    public class SendServerResponseCommand : BaseMessageHandler
    {
        private readonly Guid _clientId;
        private readonly string _response;
        private readonly CompressionTypes _compressionType;
        private readonly EncryptionTypes _encryptionType;
        private readonly ResponseTypes _responseType;

        public SendServerResponseCommand(Guid clientId, string response, ResponseTypes responseType, CompressionTypes compressType, EncryptionTypes encryptionType)
        {
            _clientId = clientId;
            _response = response;
            _responseType = responseType;
            _compressionType = compressType;
            _encryptionType = encryptionType;
        }


        public override void Execute()
        {
            ClientConnection client = ConnectionRepository.Instance.Query(c => c.ClientId == _clientId).FirstOrDefault();
            if (client != null)
            {

                StringBuilder messageBuilder = new StringBuilder();


                // first create headers
                EncryptionHeader encryptionHeader = new EncryptionHeader()
                {
                    EncryptionType = _encryptionType
                };

                if (client.ServerAuthority == null)
                {
                    client.ServerAuthority = ServerAuthorityFactory.CreateServerAuthority();
                }

                MessageHeader messageHeader = new MessageHeader()
                {
                    CompressionType = _compressionType,
                    EncryptionHeader = encryptionHeader
                };

                ResponseHeader responseHeader = new ResponseHeader()
                {
                    ResponseType = _responseType,
                    MessageHeader = messageHeader
                };

                // send response header first
                messageBuilder.Append(XmlSerializationHelper.Serialize<ResponseHeader>(responseHeader));

                // now send actual response
                //messageBuilder.Append(_response);

                MSMQQueueWrapper.QueueCommand(
                    new SendMessageCommand(_clientId, messageBuilder.ToString())
                    );

                string message = _response;

                if (_encryptionType != EncryptionTypes.None)
                {
                    byte[] publicKeyEncoded = client.RequestHeader.MessageHeader.EncryptionHeader.PublicKey;

                    DHPublicKeyParameters publicKey = new DHPublicKeyParameters(
                        ((DHPublicKeyParameters)PublicKeyFactory.CreateKey(publicKeyEncoded)).Y, client.ServerAuthority.Parameters);

                    BigInteger agreementValue = client.ServerAuthority.GenerateAgreementValue(publicKey);

                    RijndaelCrypto crypto = new RijndaelCrypto();
                    message = crypto.Encrypt(message, agreementValue.ToString(16));
                }

                MSMQQueueWrapper.QueueCommand(
                    new SendMessageCommand(_clientId, message)
                    );
            }

        }
    }
}
