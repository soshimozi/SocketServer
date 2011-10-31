using System;
using System.Linq;
using SocketService.Framework.Client.SharedObjects;
using SocketService.Framework.Data;
using SocketService.Framework.Messaging;
using SocketService.Actions;
using SocketService.Repository;
using SocketService.Framework.Client.Serialize;
using SocketService.Framework.Client.Event;

namespace SocketService.Command
{
    [Serializable]
    internal class CreateRoomVariableCommand : BaseMessageHandler
    {
        private readonly Guid _clientId;
        private readonly string _name;
        private readonly long _roomId;
        private readonly SharedObject _so;
        private readonly int _zoneId;

        public CreateRoomVariableCommand(Guid clientId, int zoneId, long roomId, string name, SharedObject so)
        {
            _clientId = clientId;
            _zoneId = zoneId;
            _roomId = roomId;
            _name = name;
            _so = so;
        }

        public override void Execute()
        {
            var room = RoomRepository.Instance.Find(_roomId);
            if (room == null) return;

            RoomActionEngine.Instance.CreateRoomVariable(room, _name, ObjectSerialize.Serialize(_so));

            MSMQQueueWrapper.QueueCommand(
                new BroadcastObjectCommand(
                    room.Users.Select( u => u.ClientKey ).ToArray(),
                    new RoomVariableUpdateEvent() { Action = RoomVariableUpdateAction.Add, Name = _name, RoomId = _roomId, Value = _so, ZoneId = _zoneId }
                    )
                );

        }
    }
}