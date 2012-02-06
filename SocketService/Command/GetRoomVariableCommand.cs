using System;
using System.Linq;
using SocketServer.Core.Data;
using SocketServer.Core.Messaging;
using SocketServer.Repository;
using SocketServer.Shared;
using SocketServer.Shared.Response;

namespace SocketServer.Command
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
            Room room = RoomRepository.Instance.Find(_roomId);
            if (room != null)
            {
                var var = room.RoomVariables.FirstOrDefault( 
                    target => target.Id == _roomId);

                object so = null;
                if( var != null)
                    so = var.Value;

                MSMQQueueWrapper.QueueCommand(
                    new SendMessageCommand<GetRoomVariableResponse>(_clientId,
                        new GetRoomVariableResponse { ZoneId = _zoneId, RoomId = room.Id, Name = _name, Value = so })
                );
            }
        }
    }
}
