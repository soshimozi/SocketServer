using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.ServiceHandlerLib;
using SocketService.Framework.Request;
using SocketService.Framework.Messaging;
using SocketService.Command;

namespace SocketService.Handler
{
    [Serializable]
    [ServiceHandlerType(typeof(DeleteRoomVariableRequest))]
    public class DeleteRoomVariableRequestHandler : BaseHandler<DeleteRoomVariableRequest, Guid>
    {
        public override bool HandleRequest(DeleteRoomVariableRequest request, Guid state)
        {
            if (request != null)
            {
                MSMQQueueWrapper.QueueCommand(
                    new DeleteRoomVariableCommand(state, request.ZoneId, request.RoomId, request.Name)
                );

                return true;

            }

            return false;
        }


    }
}
