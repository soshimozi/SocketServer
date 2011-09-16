using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Net.Sockets;
using SocketService.Serial;
using System.Net.Sockets;
using SocketService.Net;
using SocketService.Messaging;

namespace SocketService.Command
{
    [Serializable]
    class SendObjectCommand : BaseCommand
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
