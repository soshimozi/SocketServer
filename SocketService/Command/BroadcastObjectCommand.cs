using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Serial;
using SocketService.Net.Sockets;
using SocketService.Net;
using SocketService.Framework.Messaging;

namespace SocketService.Command
{
    [Serializable]
    class BroadcastObjectCommand : BaseMessageHandler
    {
        private readonly Guid [] _clientList;
        private readonly byte[] _data;

        public BroadcastObjectCommand(Guid [] clientList, object graph)
        {
            _clientList = clientList;
            _data = ObjectSerialize.Serialize(graph);
        }

        public override void Execute()
        {
            foreach (Guid clientId in _clientList)
            {
                ZipSocket connection = SocketRepository.Instance.FindByClientId(clientId);
                if (connection != null)
                {
                    try
                    {
                        connection.SendData(_data);
                    }
                    catch (Exception ex)
                    {
                        // TODO: Log exception here
                    }
                }
            }
        }
    }
}
