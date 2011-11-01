using System;
using SocketService.Core.Messaging;
using SocketService.Net;
using SocketService.Shared;
using SocketService.Shared.Sockets;

namespace SocketService.Command
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