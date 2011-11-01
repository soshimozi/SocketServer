using System;
using SocketService.Client.Core.Request;
using SocketService.Command;
using SocketService.Core.Messaging;
using SocketService.Core.ServiceHandlerLib;

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