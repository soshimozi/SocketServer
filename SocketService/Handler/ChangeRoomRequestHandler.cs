using System;
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
    [ServiceHandlerType(typeof(CreateRoomRequest))]
    class ChangeRoomRequestHandler : BaseHandler<CreateRoomRequest, Guid>
    {
        public override bool HandleRequest(CreateRoomRequest request, Guid state)
        {
            if (request != null)
            {
                string roomName = request.RoomName;

                MSMQQueueWrapper.QueueCommand(
                    new CreateRoomCommand(state, roomName)
                );

                return true;

            }

            return false;
        }
    }
}
