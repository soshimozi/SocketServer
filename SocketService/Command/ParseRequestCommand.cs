using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.Messaging;
using SocketService.Framework.Request;
using SocketService.Framework.Client.Serialize;
using SocketService.Framework.ServiceHandlerLib;
using SocketService.Crypto;
using SocketService.Net.Client;

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
            ClientRequest request = (ClientRequest)ObjectSerialize.Deserialize(_serialized);
            object payload = DecryptRequest(request);
        
            Type handlerType = payload.GetType();
            var handlerList = ServiceHandlerLookup.Instance.GetHandlerListByType(handlerType);

            MSMQQueueWrapper.QueueCommand(
                new HandleClientRequestCommand(_clientId, payload, handlerList)
            );
        }

        private object DecryptRequest(ClientRequest request)
        {
            // switch on encryption type, and create a decryptor for that type
            // with the remote private key and iv as salt
            AlgorithmType algorithm = AlgorithmType.AES;

            switch (request.Encryption)
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


            if (algorithm != AlgorithmType.None)
            {
                Connection connection = ConnectionRepository.Instance.FindConnectionByClientId(_clientId);
                if (connection != null)
                {
                    DiffieHellmanKey privateKey = connection.Provider.CreatePrivateKey(connection.RemotePublicKey);
                    using (Wrapper cryptoWrapper = Wrapper.CreateDecryptor(algorithm,
                                                                        privateKey.ToByteArray(),
                                                                        request.EncryptionPublicKey))
                    {
                        return ObjectSerialize.Deserialize(cryptoWrapper.Decrypt(request.RequestData));
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return ObjectSerialize.Deserialize(request.RequestData);
            }
        }
    }
}
