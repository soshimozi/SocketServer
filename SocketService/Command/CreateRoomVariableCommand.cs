using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.Messaging;
using SocketService.Framework.SharedObjects;
using SocketService.Actions;
using SocketService.Framework.Data;
using SocketService.Framework.Client.Event;

namespace SocketService.Command
{
    [Serializable]
    class CreateRoomVariableCommand : BaseMessageHandler
    {
        private readonly int _zoneId;
        private readonly int _roomId;
        private readonly string _name;
        private readonly RoomVariable _so;
        private readonly Guid _clientId;
        public CreateRoomVariableCommand(Guid clientId, int zoneId, int roomId, string name, RoomVariable so)
        {
            _zoneId = zoneId;
            _roomId = roomId;
            _name = name;
            _so = so;
            _clientId = clientId;
        }

        public override void Execute()
        {
            Room room = RoomActionEngine.Instance.Find(_roomId);

            if (room != null)
            {
                //RoomActionEngine.Instance.CreateRoomVariable(_room, _name, _so);
                room.AddVariable(_name, _so);

                MSMQQueueWrapper.QueueCommand(
                    new BroadcastObjectCommand(
                        UserRepository.Instance.FindClientKeysByRoom(room.Name).ToArray(),
                        new RoomVariableUpdateEvent()
                        {
                            ZoneId = _zoneId,
                            RoomId = room.Id,
                            Name = _name,
                            Variable = _so,
                            Action = RoomVariableUpdateAction.Add
                        }
                    )
                );

                //MSMQQueueWrapper.QueueCommand(
                //    new SendObjectCommand(
                //        _clientId,
                //        new CreateRoomVariableResponse()
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
}
