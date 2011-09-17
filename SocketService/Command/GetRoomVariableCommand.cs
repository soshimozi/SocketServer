using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.SharedObjects;
using SocketService.Response;
using SocketService.Framework.Messaging;
using SocketService.Framework.Actions;

namespace SocketService.Command
{
    [Serializable]
    public class GetRoomVariableCommand : BaseMessageHandler
    {
        private readonly string _room;
        private readonly string _name;
        private readonly Guid _clientId;

        public GetRoomVariableCommand(Guid clientId, string room, string name)
        {
            _clientId = clientId;
            _room = room;
            _name = name;
        }

        public override void Execute()
        {
            ServerObject so = RoomActionEngine.Instance.GetRoomVariable(_room, _name);

            MSMQQueueWrapper.QueueCommand(
                new SendObjectCommand(_clientId,
                    new GetRoomVariableResponse() { ServerObject = so })
            );
        }
    }
}
