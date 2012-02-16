using System;
using System.Linq;
using System.Collections.Generic;
using SocketServer.Shared.Header;
using SocketServer.Shared.Reflection;
using SocketServer.Shared;
using SocketServer.Shared.Serialization;
using System.Reflection;
using log4net;
using SocketServer.Shared.Request;
using SocketServer.Repository;
using SocketServer.Net.Client;
using SocketServer.Crypto;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using SocketServer.Shared.Network;

namespace SocketServer.Command
{
    [Serializable]
    public class HandleClientRequestCommand : BaseCommandHandler
    {
        private readonly Guid _clientId;
        private readonly string _request;
        private readonly string _requestType;
        private readonly string _handlerName;

        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public HandleClientRequestCommand(Guid id, string handlerName, string requestType, string request)
        {
            _clientId = id;
            _handlerName = handlerName;
            _requestType = requestType;
            _request = request;
        }

        public override void Execute()
        {
            ClientConnection client = ConnectionRepository.Instance.Query( c => c.ClientId == _clientId ).FirstOrDefault();

            if (client != null)
            {
                string requestString = ProcessRawRequest(client, _request);
                
                object requestObject
                    = DeSerializeRequest(
                        _requestType,
                        ProcessRawRequest(client, _request));

                if (requestObject != null)
                {
                    ServiceHandlerRepository.Instance.InvokeHandler(_handlerName, requestObject, _clientId);
                }
            }
        }

        private object DeSerializeRequest(string requestTypeString, string requestString)
        {
            Type requestType = ReflectionHelper.FindType(requestTypeString);
            if (requestType != null)
            {
                return XmlSerializationHelper.DeSerialize(requestString, requestType);
            }
            return null;
        }

        private string ProcessRawRequest(ClientConnection client, string requestString)
        {
            if (client.RequestHeader.MessageHeader.CompressionType != CompressionTypes.None)
            {
                // decompress
            }
            
            if (client.RequestHeader.MessageHeader.EncryptionHeader.EncryptionType != EncryptionTypes.None)
            {
                // decrypt
                byte [] publicKeyEncoded = client.RequestHeader.MessageHeader.EncryptionHeader.PublicKey;

                DHPublicKeyParameters publicKey = new DHPublicKeyParameters(
                    ((DHPublicKeyParameters)PublicKeyFactory.CreateKey(publicKeyEncoded)).Y, client.ServerAuthority.Parameters);

                BigInteger agreementValue = client.ServerAuthority.GenerateAgreementValue(publicKey);

                RijndaelCrypto crypto = new RijndaelCrypto();
                return crypto.Decrypt(requestString, agreementValue.ToString(16));
            }

            return requestString;
        }

    }
}
