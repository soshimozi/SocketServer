using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.Messaging;
using SocketService.Framework.Data;
using SocketService.Actions;
using SocketService.Framework.Client.Event;
using SocketService.Framework.Client.Response;

namespace SocketService.Command
{
    [Serializable]
    public class DeleteRoomVariableCommand : BaseMessageHandler
    {
        private readonly Guid _clientId;
        private readonly string _roomName;
        private readonly string _name;
        public DeleteRoomVariableCommand(Guid ClientId, string RoomName, string Name)
        {
            _clientId = ClientId;
            _roomName = RoomName;
            _name = Name;
        }

        public override void Execute()
        {

            Room room = RoomActionEngine.Instance.GetRoomByName(_roomName);
            RoomActionEngine.Instance.DeleteRoomVariable(room.Id, _name);

            MSMQQueueWrapper.QueueCommand(
                new BroadcastObjectCommand(
                    UserRepository.Instance.FindClientKeysByRoom(_roomName).ToArray(),
                    new RoomVariableUpdateEvent()
                    {
                        RoomId = room.Id,
                        Name = _name,
                        Action = RoomVariableUpdateAction.Delete
                    }
                )
            );

            //MSMQQueueWrapper.QueueCommand(
            //    new SendObjectCommand(
            //        _clientId,
            //        new DeleteRoomVariableResponse()
            //        {
            //            Room = _roomName,
            //            Name = _name,
            //        }
            //    )
            //);

        }
    }
}
