using System;
using System.Linq;
using System.Reflection;
using log4net;
using SocketServer.Core.Messaging;
using SocketServer.Net;
using SocketServer.Shared;
using SocketServer.Shared.Interop.Java;
using SocketServer.Shared.Serialization;

namespace SocketServer.Command
{
    [Serializable]
    class BroadcastMessageCommand<T> : BaseMessageHandler where T : class
    {
        private readonly Guid [] _clientList;
        private readonly string _message;

        private readonly static ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public BroadcastMessageCommand(Guid[] clientList, T graph)
        {
            _clientList = clientList;
            _message = XmlSerializationHelper.Serialize<T>(graph);

            //_data = ObjectSerialize.Serialize(graph);
        }

        public override void Execute()
        {
            foreach (var connection in _clientList.Select(clientId => SocketRepository.Instance.FindByClientId(clientId)).Where(connection => connection != null))
            {
                try
                {
                    connection.SendData(_message.SerializeUTF());
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
        }
    }
}
