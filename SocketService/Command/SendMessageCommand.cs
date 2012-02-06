using System;
using SocketServer.Core.Messaging;
using SocketServer.Net;
using SocketServer.Shared;
using SocketServer.Shared.Sockets;
using SocketServer.Shared.Interop.Java;
using System.Xml.Serialization;
using System.Text;
using System.IO;
using SocketServer.Shared.Serialization;

namespace SocketServer.Command
{
    [Serializable]
    internal class SendMessageCommand<T> : BaseMessageHandler where T : class
    {
        private readonly Guid _clientId;
        private readonly string _message;

        public SendMessageCommand(Guid clientId, T graph)
        {
            _clientId = clientId;
            _message = XmlSerializationHelper.Serialize<T>(graph);

            //_message = message;

            //_data = ObjectSerialize.Serialize(graph);
        }

        public override void Execute()
        {
            ZipSocket connection = SocketRepository.Instance.FindByClientId(_clientId);
            if (connection != null)
            {
                connection.SendData(_message.SerializeUTF());
            }
        }
    }
}