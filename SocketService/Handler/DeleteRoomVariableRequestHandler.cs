using System;
using SocketServer.Command;
using SocketServer.Core.Messaging;
using SocketServer.Core.ServiceHandlerLib;
using SocketServer.Shared.Request;

namespace SocketServer.Handler
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