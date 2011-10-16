using System;
using System.Linq;
using System.Reflection;
using SocketService.Framework.Messaging;
using SocketService.Framework.Client.Serialize;
using SocketService.Net;
using log4net;

namespace SocketService.Command
{
    [Serializable]
    class BroadcastObjectCommand : BaseMessageHandler
    {
        private readonly Guid [] _clientList;
        private readonly byte[] _data;

        private readonly static ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public BroadcastObjectCommand(Guid [] clientList, object graph)
        {
            _clientList = clientList;
            _data = ObjectSerialize.Serialize(graph);
        }

        public override void Execute()
        {
            foreach (var connection in _clientList.Select(clientId => SocketRepository.Instance.FindByClientId(clientId)).Where(connection => connection != null))
            {
                try
                {
                    connection.SendData(_data);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
        }
    }
}
