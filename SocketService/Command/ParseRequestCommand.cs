using System;
using SocketService.Client.Core.Request;
using SocketService.Core.Crypto;
using SocketService.Core.Messaging;
using SocketService.Net.Client;
using SocketService.Repository;
using SocketService.Shared;

namespace SocketService.Command
{
    [Serializable]
    class ParseRequestCommand : BaseMessageHandler
    {
        private readonly byte[] _serialized;
        private readonly Guid _clientId;

        public ParseRequestCommand(Guid clientId, byte[] serialized)
        {
            _serialized = serialized;
            _clientId = clientId;
        }

        public override void Execute()
        {
            var requestWrapper = ObjectSerialize.Deserialize<ClientRequestWrapper>(_serialized);
            var payload = DecryptRequest(requestWrapper);
        
            var handlerType = payload.GetType();
            var handlerList = ServiceHandlerLookup.Instance.GetHandlerListByType(handlerType);

            MSMQQueueWrapper.QueueCommand(
                new HandleClientRequestCommand(_clientId, payload, handlerList)
            );
        }

        private object DecryptRequest(ClientRequestWrapper requestWrapper)
        {
            // switch on encryption type, and create a decryptor for that type
            // with the remote private key and iv as salt
            var algorithm = AlgorithmType.AES;

            switch (requestWrapper.Encryption)
            {
                case EncryptionType.DES:
                    algorithm = AlgorithmType.DES;
                    break;

                case EncryptionType.TripleDES:
                    algorithm = AlgorithmType.TripleDES;
                    break;

                case EncryptionType.None:
                    algorithm = AlgorithmType.None;
                    break;
            }


            if (algorithm == AlgorithmType.None)
            {
                return ObjectSerialize.Deserialize(requestWrapper.RequestData);
            }
            
            var connection = ConnectionRepository.Instance.FindConnectionByClientId(_clientId);
            if (connection == null)
            {
                return null;
            }
            
            var privateKey = connection.Provider.CreatePrivateKey(connection.RemotePublicKey);
            using (var cryptoWrapper = Wrapper.CreateDecryptor(algorithm,
                                                                   privateKey.ToByteArray(),
                                                                   requestWrapper.EncryptionPublicKey))
            {
                return ObjectSerialize.Deserialize(cryptoWrapper.Decrypt(requestWrapper.RequestData));
            }
        }
    }
}
