using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using SocketService.Framework.Net.Sockets;
using SocketService.Framework.Net;
using SocketService.Framework.Messaging;
using SocketService.Framework.Serialize;

namespace SocketService.Framework.Command
{
    [Serializable]
    public class SendObjectCommand : BaseMessageHandler
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
