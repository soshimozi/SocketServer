using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.Messaging;
using SocketService.Framework.Client.Serialize;
using SocketService.Framework.Client.Sockets;
using SocketService.Net;

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
                    catch (Exception)
                    {
                        // TODO: Log exception here
                    }
                }
            }
        }
    }
}
