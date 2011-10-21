using System;
using System.Linq;
using SocketService.Framework.Client.Event;
using SocketService.Framework.Client.SharedObjects;
using SocketService.Framework.Data;
using SocketService.Framework.Messaging;
using SocketService.Repository;

namespace SocketService.Command
{
    [Serializable]
    public class UpdateRoomVariableCommand : BaseMessageHandler
    {
        private readonly Guid _clientId;
        private readonly string _name;
        private readonly int _roomId;
        private readonly SharedObject _so;
        private readonly int _zoneId;

        public UpdateRoomVariableCommand(Guid clientId, int zoneId, int roomId, string name, SharedObject value)
        {
            _zoneId = zoneId;
            _roomId = roomId;
            _name = name;
            _so = value;
            _clientId = clientId;
        }


        public override void Execute()
        {
            Room room = RoomRepository.Instance.Find(_roomId);

            //RoomActionEngine.Instance.UpdateRoomVariable(room.Id, _name, _so);

            MSMQQueueWrapper.QueueCommand(
                new BroadcastObjectCommand(
                    room.Users.Select(u => u == null ? new Guid() : u.ClientKey).ToArray(),
                    new RoomVariableUpdateEvent
                        {
                            ZoneId =  _zoneId,
                            RoomId = room.Id,
                            Name = _name,
                            Value = _so,
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