using System;
using SocketService.Client.Core.Request;
using SocketService.Command;
using SocketService.Core.Messaging;
using SocketService.Core.ServiceHandlerLib;

namespace SocketService.Handler
{
    [Serializable]
    [ServiceHandlerType(typeof (CreateRoomVariableRequest))]
    public class CreateRoomVariableRequestHandler : BaseHandler<CreateRoomVariableRequest, Guid>
    {
        public override bool HandleRequest(CreateRoomVariableRequest request, Guid state)
        {
            MSMQQueueWrapper.QueueCommand(
                new CreateRoomVariableCommand(state, request.ZoneId, request.RoomId, request.Name, request.Value)
                );

            return true;
        }
    }
}