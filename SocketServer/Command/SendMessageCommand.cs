using System;
using SocketServer.Messaging;
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
    internal class SendMessageCommand : BaseMessageHandler
    {
        private readonly Guid _clientId;
        private readonly string _message;

        public SendMessageCommand(Guid clientId, string message)
        {
            _clientId = clientId;
            _message = message;
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