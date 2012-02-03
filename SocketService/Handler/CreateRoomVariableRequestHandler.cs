using System;
using SocketServer.Command;
using SocketServer.Core.Messaging;
using SocketServer.Core.ServiceHandlerLib;
using SocketServer.Shared.Request;

namespace SocketServer.Handler
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