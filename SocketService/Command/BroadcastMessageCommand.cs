using System;
using System.Linq;
using System.Reflection;
using log4net;
using SocketServer.Net;
using SocketServer.Shared;
using SocketServer.Shared.Serialization;
using SocketServer.Shared.Header;
using SocketServer.Net.Client;

namespace SocketServer.Command
{
    [Serializable]
    class BroadcastMessageCommand : BaseCommandHandler
    {
        private readonly static ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Guid[] _clientList;
        private readonly string _response;
        private readonly CompressionTypes _compressionType;
        private readonly EncryptionTypes _encryptionType;
        private readonly ResponseTypes _responseType;

        public BroadcastMessageCommand(Guid[] clientList, string response, ResponseTypes responseType, MessageHeader header)
        {
            _clientList = clientList;
            _response = response;
            _responseType = responseType;
            _compressionType = header.CompressionType;
            _encryptionType = header.EncryptionHeader.EncryptionType;
        }

        public override void Execute()
        {
            foreach (var connection in ConnectionRepository.Instance.Query(c => _clientList.Contains(c.ClientId)))
            {
                try
                {
                    ResponseHeader responseHeader = ResponseBuilder.BuildResponseHeader(_encryptionType, _compressionType, _responseType);

                    //connection. .ClientSocket.SendData(
                    //    XmlSerializationHelper
                    //    .Serialize<ResponseHeader>(responseHeader)
                    //    .SerializeUTF());


                    //connection.ClientSocket.SendData(
                    //        ResponseBuilder.ProcessResponse(
                    //            connection.ServerAuthority,
                    //            connection
                    //            .RequestHeader
                    //            .MessageHeader
                    //            .EncryptionHeader
                    //            .PublicKey,
                    //            responseHeader,
                    //            _response).SerializeUTF());
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
        }
    }
}
