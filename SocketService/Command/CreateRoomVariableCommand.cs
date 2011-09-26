using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.Messaging;
using SocketService.Framework;
using SocketService.Framework.SharedObjects;

namespace SocketService.Command
{
    [Serializable]
    class CreateRoomVariableCommand : BaseMessageHandler
    {
        private readonly Guid _clientId;
        private readonly int _zoneId;
        private readonly int _roomId;
        private readonly string _name;
        private readonly SharedObject _so;
        public CreateRoomVariableCommand(Guid clientId, int zoneId, int roomId, string name, SharedObject so)
        {
            _clientId = clientId;
            _zoneId = zoneId;
            _roomId = roomId;
            _name = name;
            _so = so;
        }

        public override void Execute()
        {
            //RoomActionEngine.Instance.CreateRoomVariable(_room, _name, _so);

            // TODO: Send RoomVariableUpdateEvent to all users in room
        }
    }
}
