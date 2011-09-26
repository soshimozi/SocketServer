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
        private readonly int _zoneId;
        private readonly int _roomId;
        private readonly string _name;
        public DeleteRoomVariableCommand(Guid ClientId, int ZoneId, int RoomId, string Name)
        {
            _zoneId = ZoneId;
            _clientId = ClientId;
            _roomId = RoomId;
            _name = Name;
        }

        public override void Execute()
        {

            //Room room = RoomActionEngine.Instance.GetRoomByName(_roomName);
            //RoomActionEngine.Instance.DeleteRoomVariable(room.Id, _name);

            //MSMQQueueWrapper.QueueCommand(
            //    new BroadcastObjectCommand(
            //        UserRepository.Instance.FindClientKeysByRoom(_roomName).ToArray(),
            //        new RoomVariableUpdateEvent()
            //        {
            //            RoomId = room.Id,
            //            Name = _name,
            //            Action = RoomVariableUpdateAction.Delete
            //        }
            //    )
            //);

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
