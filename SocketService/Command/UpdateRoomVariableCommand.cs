using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.Messaging;
using SocketService.Framework.SharedObjects;
using SocketService.Actions;
using SocketService.Framework.Client.Event;
using SocketService.Framework.Data;
using SocketService.Framework.Client.Response;

namespace SocketService.Command
{
    [Serializable]
    public class UpdateRoomVariableCommand : BaseMessageHandler
    {
        private readonly string _roomName;
        private readonly string _name;
        private readonly RoomVariable _so;
        private readonly Guid _clientId;

        public UpdateRoomVariableCommand(Guid clientId, string Room, string Name, RoomVariable Value)
        {
            _roomName = Room;
            _name = Name;
            _so = Value;
            _clientId = clientId;
        }


        public override void Execute()
        {
            Room room = RoomActionEngine.Instance.GetRoomByName(_roomName);
            RoomActionEngine.Instance.UpdateRoomVariable(room.Id, _name, _so);

            MSMQQueueWrapper.QueueCommand(
                new BroadcastObjectCommand(
                    UserRepository.Instance.FindClientKeysByRoom(_roomName).ToArray(),
                    new RoomVariableUpdateEvent()
                    {
                        RoomId = room.Id,
                        Name = _name,
                        Variable = _so,
                        Action = RoomVariableUpdateAction.Update
                    }
                )
            );

            //MSMQQueueWrapper.QueueCommand(
            //    new SendObjectCommand(
            //        _clientId,
            //        new UpdateRoomVariableResponse()
            //        {
            //            Room = _roomName,
            //            Name = _name,
            //            Value = _so,
            //        }
            //    )
            //);

        }
    }
}
