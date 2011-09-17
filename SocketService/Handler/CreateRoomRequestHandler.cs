using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using SocketService.Framework.Messaging;
using SocketService.Framework.ServiceHandler;
using SocketService.Framework.Client.Request;
using SocketService.Framework.Command;

namespace SocketService.Handler
{
    [Serializable()]
    [ServiceHandlerType(typeof(ChangeRoomRequest))]
    class CreateRoomRequestHandler : BaseHandler<ChangeRoomRequest, Guid>
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
