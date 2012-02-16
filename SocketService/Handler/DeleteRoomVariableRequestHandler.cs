using System;
using SocketServer.Command;
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

        public void HandleRequest(DeleteRoomVariableRequest request, Shared.Network.ClientConnection connection)
        {
            throw new NotImplementedException();
        }
    }
}