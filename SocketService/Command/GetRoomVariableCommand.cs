﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.Messaging;
using SocketService.Framework.SharedObjects;
using SocketService.Actions;
using SocketService.Framework.Client.Response;

namespace SocketService.Command
{
    [Serializable]
    public class GetRoomVariableCommand : BaseMessageHandler
    {
        private readonly string _room;
        private readonly string _name;
        private readonly Guid _clientId;

        public GetRoomVariableCommand(Guid clientId, string room, string name)
        {
            _clientId = clientId;
            _room = room;
            _name = name;
        }

        public override void Execute()
        {
            RoomVariable so = RoomActionEngine.Instance.GetRoomVariable(_room, _name);

            MSMQQueueWrapper.QueueCommand(
                new SendObjectCommand(_clientId,
                    new GetRoomVariableResponse() { Room = _room, Name = _name, RoomVariable = so })
            );
        }
    }
}
