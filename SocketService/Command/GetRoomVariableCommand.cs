using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Messaging;
using SocketService.Actions;
using SocketService.SharedObjects;
using SocketService.Response;

namespace SocketService.Command
{
    [Serializable]
    public class GetRoomVariableCommand : BaseCommand
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
