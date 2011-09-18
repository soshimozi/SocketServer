﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using SocketService.Command;
using SocketService.Framework.Messaging;
using SocketService.Framework.ServiceHandlerLib;
using SocketService.Framework.Request;

namespace SocketService
{
    [Serializable()]
    [ServiceHandlerType(typeof(ChangeRoomRequest))]
    class ChangeRoomRequestHandler : BaseHandler<ChangeRoomRequest, Guid>
    {
        public override bool HandleRequest(ChangeRoomRequest request, Guid state)
        {
            if (request != null)
            {
                string roomName = request.RoomName;

                MSMQQueueWrapper.QueueCommand(
                    new ChangeRoomCommand(state, roomName, true)
                );

                return true;

            }

            return false;
        }
    }
}