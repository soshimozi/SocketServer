using System;
using SocketService.Framework.Client.Serialize;
using SocketService.Framework.Client.Sockets;
using SocketService.Framework.Messaging;
using SocketService.Net;

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