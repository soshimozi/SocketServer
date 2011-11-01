using System;
using SocketService.Command;
using SocketService.Core.Messaging;
using SocketService.Core.ServiceHandlerLib;
using SocketService.Shared.Request;

namespace SocketService.Handler
{
    [Serializable]
    [ServiceHandlerType(typeof (UpdateRoomVariableRequest))]
    public class UpdateRoomVariableRequestHandler : BaseHandler<UpdateRoomVariableRequest, Guid>
    {
        public override bool HandleRequest(UpdateRoomVariableRequest request, Guid state)
        {
            MSMQQueueWrapper.QueueCommand(
                new UpdateRoomVariableCommand(state, request.ZoneId, request.RoomId, request.Name, request.Value)
                );

            return true;
        }
    }
}