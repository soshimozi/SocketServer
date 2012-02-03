using System;
using SocketServer.Command;
using SocketServer.Core.Messaging;
using SocketServer.Core.ServiceHandlerLib;
using SocketServer.Shared.Request;

namespace SocketServer.Handler
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