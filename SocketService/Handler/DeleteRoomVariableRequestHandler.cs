using System;
using SocketService.Command;
using SocketService.Core.Messaging;
using SocketService.Core.ServiceHandlerLib;
using SocketService.Shared.Request;

namespace SocketService.Handler
{
    [Serializable]
    [ServiceHandlerType(typeof (DeleteRoomVariableRequest))]
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