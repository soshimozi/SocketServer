using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using SocketService.Framework.Messaging;
using SocketService.Framework.Client.Sockets;
using SocketService.Net;
using SocketService.Framework.Client.Serialize;

namespace SocketService.Command
{
    [Serializable]
    class SendObjectCommand : BaseMessageHandler
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
