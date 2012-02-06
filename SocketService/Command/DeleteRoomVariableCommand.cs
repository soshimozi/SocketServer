using System;
using SocketServer.Messaging;

namespace SocketServer.Command
{
    [Serializable]
    public class DeleteRoomVariableCommand : BaseMessageHandler
    {
        private readonly Guid _clientId;
        private readonly int _zoneId;
        private readonly int _roomId;
        private readonly string _name;
        public DeleteRoomVariableCommand(Guid clientId, int zoneId, int roomId, string name)
        {
            _zoneId = zoneId;
            _clientId = clientId;
            _roomId = roomId;
            _name = name;
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
