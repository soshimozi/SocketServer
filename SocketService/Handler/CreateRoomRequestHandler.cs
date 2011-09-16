using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.ServiceHandler;
using System.ComponentModel.Composition;
using SocketService.Request;
using SocketService.Messaging;
using SocketService.Command;

namespace SocketService.Handler
{
    [Serializable()]
    [ServiceHandlerType(typeof(ChangeRoomRequest))]
    [Export(typeof(IServiceHandler))]
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
