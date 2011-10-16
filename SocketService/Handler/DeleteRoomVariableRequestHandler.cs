using System;
using SocketService.Command;
using SocketService.Framework.Client.Request;
using SocketService.Framework.Messaging;
using SocketService.Framework.ServiceHandlerLib;

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