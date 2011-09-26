using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.Messaging;
using SocketService.Framework.SharedObjects;
using SocketService.Actions;
using SocketService.Framework.Client.Response;
using SocketService.Framework;
using SocketService.Framework.Data;
using SocketService.Framework.Client.Serialize;

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
                RoomVariable var = room.RoomVariables.FirstOrDefault( 
                    new Func<RoomVariable, bool>(
                        (target) =>
                        {
                            return target.Id == _roomId;
                        }
                    )
                );

                SharedObject so = new SharedObject();
                so.SetElementValue("", ObjectSerialize.Deserialize(var.Value), SharedObjectDataType.BzObject);

                MSMQQueueWrapper.QueueCommand(
                    new SendObjectCommand(_clientId,
                        new GetRoomVariableResponse() { ZoneId = _zoneId, RoomId = room.Id, Name = _name, Value = so })
                );
            }
        }
    }
}
