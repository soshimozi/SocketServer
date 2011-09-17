using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.Messaging;
using SocketService.Framework.Net.Sockets;
using SocketService.Framework.Serialize;
using SocketService.Framework.Net;

namespace SocketService.Framework.Command
{
    [Serializable]
    public class BroadcastObjectCommand : BaseMessageHandler
    {
        private readonly Guid [] _clientList;
        private readonly byte[] _data;

        /// <summary>
        /// Initializes a new instance of the <see cref="BroadcastObjectCommand"/> class.
        /// </summary>
        /// <param name="clientList">The client list.</param>
        /// <param name="graph">The graph.</param>
        public BroadcastObjectCommand(Guid [] clientList, object graph)
        {
            _clientList = clientList;
            _data = ObjectSerialize.Serialize(graph);
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        public override void Execute()
        {
            foreach (Guid clientId in _clientList)
            {
                ZipSocket connection = SocketRepository.Instance.FindByClientId(clientId);
                if (connection != null)
                {
                    connection.SendData(_data);
                }
            }
        }
    }
}
