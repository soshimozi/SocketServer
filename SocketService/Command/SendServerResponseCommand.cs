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
using SocketServer.Shared.Interop.Java;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using SocketServer.Shared.Sockets;
using SocketServer.Net;

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
                ResponseHeader responseHeader = ResponseBuilder.BuildResponseHeader(_encryptionType, _compressionType, _responseType);
                ZipSocket connection = SocketRepository.Instance.FindByClientId(_clientId);
                if (connection != null)
                {
                    connection.SendData(
                        XmlSerializationHelper
                        .Serialize<ResponseHeader>(responseHeader)
                        .SerializeUTF());


                    connection.SendData(
                            ResponseBuilder.ProcessResponse(
                                client.ServerAuthority,
                                client
                                .RequestHeader
                                .MessageHeader
                                .EncryptionHeader
                                .PublicKey,
                                responseHeader,
                                _response).SerializeUTF());
                }
            }
        }
    }
}
