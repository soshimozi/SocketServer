using System;
using SocketServer.Command;
using SocketServer.Core.Messaging;
using SocketServer.Shared.Request;

namespace SocketServer.Handler
{
    public class UpdateRoomVariableRequestHandler : IRequestHandler<UpdateRoomVariableRequest>
    {
        public void HandleRequest(UpdateRoomVariableRequest request, Guid state)
        {
            MSMQQueueWrapper.QueueCommand(
                new UpdateRoomVariableCommand(state, request.ZoneId, request.RoomId, request.Name, request.Value)
                );

        }
    }
}