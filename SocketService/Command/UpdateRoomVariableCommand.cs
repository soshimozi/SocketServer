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
        private readonly int _zoneId;
        private readonly int _roomId;
        private readonly string _name;
        private readonly SharedObject _so;
        private readonly Guid _clientId;

        public UpdateRoomVariableCommand(Guid clientId, int ZoneId, int RoomId, string Name, SharedObject Value)
        {
            _zoneId = ZoneId;
            _roomId = RoomId;
            _name = Name;
            _so = Value;
            _clientId = clientId;
        }


        public override void Execute()
        {
            Room room = RoomActionEngine.Instance.GetRoomById(_roomId);
            //RoomActionEngine.Instance.UpdateRoomVariable(room.Id, _name, _so);

            MSMQQueueWrapper.QueueCommand(
                new BroadcastObjectCommand(
                    room.Users.Select((u) => { return u.ClientKey; }).ToArray(),
                    new RoomVariableUpdateEvent()
                    {
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
