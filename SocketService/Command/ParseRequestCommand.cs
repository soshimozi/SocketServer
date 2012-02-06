using System;
using System.Linq;
using SocketServer.Messaging;
using SocketServer.Crypto;
using SocketServer.Net.Client;
using SocketServer.Repository;
using SocketServer.Shared;
using SocketServer.Shared.Request;

namespace SocketServer.Command
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
            //var requestWrapper = ObjectSerialize.Deserialize<ClientRequest>(_serialized);
            //var payload = DecryptRequest(requestWrapper);
        
            //var handlerType = payload.GetType();
            //var handlerList = ServiceHandlerLookup.Instance.GetHandlerListByType(handlerType);

            //MSMQQueueWrapper.QueueCommand(
            //    new HandleClientRequestCommand(_clientId, payload, handlerList)
            //);
        }

        //private object DecryptRequest(ClientRequest requestWrapper)
        //{
        //    //// switch on encryption type, and create a decryptor for that type
        //    //// with the remote private key and iv as salt
        //    //var algorithm = AlgorithmType.AES;

        //    //switch (requestWrapper.Encryption)
        //    //{
        //    //    case EncryptionType.DES:
        //    //        algorithm = AlgorithmType.DES;
        //    //        break;

        //    //    case EncryptionType.TripleDES:
        //    //        algorithm = AlgorithmType.TripleDES;
        //    //        break;

        //    //    case EncryptionType.None:
        //    //        algorithm = AlgorithmType.None;
        //    //        break;
        //    //}


        //    //if (algorithm == AlgorithmType.None)
        //    //{
        //    //    return ObjectSerialize.Deserialize(requestWrapper.RequestData);
        //    //}
            
        //    //var connection = ConnectionRepository.Instance.Query( c => c.ClientId == _clientId).FirstOrDefault();
        //    //if (connection == null)
        //    //{
        //    //    return null;
        //    //}

        //    //var privateKey = connection.Provider.Agree();
        //    //using (var cryptoWrapper = CryptoManager.CreateDecryptor(algorithm, privateKey, requestWrapper.EncryptionPublicKey))
        //    //{
        //    //    return ObjectSerialize.Deserialize(cryptoWrapper.Decrypt(requestWrapper.RequestData));
        //    //}

        //    //var privateKey = connection.SecureKeyProvider.CreatePrivateKey(connection.RemotePublicKey);
        //    //using (var cryptoWrapper = CryptoManager.CreateDecryptor(algorithm,
        //    //                                                       privateKey.ToByteArray(),
        //    //                                                       requestWrapper.EncryptionPublicKey))
        //    //{
        //    //    return ObjectSerialize.Deserialize(cryptoWrapper.Decrypt(requestWrapper.RequestData));
        //    //}

        //    return null;
        //}
    }
}
