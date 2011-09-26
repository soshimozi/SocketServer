using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.Messaging;
using SocketService.Framework.SharedObjects;
using SocketService.Actions;
using SocketService.Framework.Client.Response;
using SocketService.Framework;

namespace SocketService.Command
{
    [Serializable]
    public class GetRoomVariableCommand : BaseMessageHandler
    {
        private readonly int _zoneId;
        private readonly int _roomId;
        private readonly string _name;
        private readonly Guid _clientId;

        public GetRoomVariableCommand(Guid clientId, int zoneId, int roomId, string name)
        {
            _clientId = clientId;
            _zoneId = zoneId;
            _roomId = roomId;
            _name = name;
        }

        public override void Execute()
        {
            Room room = RoomActionEngine.Instance.Find(_roomId);
            if (room != null)
            {
                RoomVariable so = RoomActionEngine.Instance.GetRoomVariable(_room, _name);

                MSMQQueueWrapper.QueueCommand(
                    new SendObjectCommand(_clientId,
                        new GetRoomVariableResponse() { Room = _room, Name = _name, Variable = so })
                );
            }
        }
    }
}
