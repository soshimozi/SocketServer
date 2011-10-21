using System;
using System.Linq;
using SocketService.Framework.Client.SharedObjects;
using SocketService.Framework.Messaging;
using SocketService.Framework.Client.Response;
using SocketService.Framework.Data;
using SocketService.Framework.Client.Serialize;
using SocketService.Repository;

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
            Room room = RoomRepository.Instance.Find(_roomId);
            if (room != null)
            {
                var var = room.RoomVariables.FirstOrDefault( 
                    target => target.Id == _roomId);

                var so = new SharedObject();
                if (var != null)
                    so.SetElementValue("", ObjectSerialize.Deserialize(var.Value), SharedObjectDataType.BzObject);

                MSMQQueueWrapper.QueueCommand(
                    new SendObjectCommand(_clientId,
                        new GetRoomVariableResponse { ZoneId = _zoneId, RoomId = room.Id, Name = _name, Value = so })
                );
            }
        }
    }
}
