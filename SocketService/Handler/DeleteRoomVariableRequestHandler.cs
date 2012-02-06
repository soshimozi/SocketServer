using System;
using SocketServer.Command;
using SocketServer.Core.Messaging;
using SocketServer.Shared.Request;

namespace SocketServer.Handler
{
    public class DeleteRoomVariableRequestHandler : IRequestHandler<DeleteRoomVariableRequest>
    {
        public void HandleRequest(DeleteRoomVariableRequest request, Guid state)
        {
            if (request != null)
            {
                MSMQQueueWrapper.QueueCommand(
                    new DeleteRoomVariableCommand(state, request.ZoneId, request.RoomId, request.Name)
                    );

            }

        }
    }
}