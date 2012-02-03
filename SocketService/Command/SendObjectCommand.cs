using System;
using SocketServer.Core.Messaging;
using SocketServer.Net;
using SocketServer.Shared;
using SocketServer.Shared.Sockets;

namespace SocketServer.Command
{
    [Serializable]
    internal class SendObjectCommand : BaseMessageHandler
    {
        private readonly Guid _clientId;
        private readonly byte[] _data;

        public SendObjectCommand(Guid clientId, object graph)
        {
            _clientId = clientId;
            _data = ObjectSerialize.Serialize(graph);
        }

        public override void Execute()
        {
            ZipSocket connection = SocketRepository.Instance.FindByClientId(_clientId);
            if (connection != null)
            {
                connection.SendData(_data);
            }
        }
    }
}