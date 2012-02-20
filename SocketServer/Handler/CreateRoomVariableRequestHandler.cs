using System;
using SocketServer.Command;
using SocketServer.Shared.Request;

namespace SocketServer.Handler
{
    public class CreateRoomVariableRequestHandler : IRequestHandler<CreateRoomVariableRequest>
    {
        public void HandleRequest(CreateRoomVariableRequest request, Guid state)
        {
            MSMQQueueWrapper.QueueCommand(
                new CreateRoomVariableCommand(state, request.ZoneId, request.RoomId, request.Name, request.Value)
                );
        }

        public void HandleRequest(CreateRoomVariableRequest request, Shared.Network.ClientConnection connection)
        {
            throw new NotImplementedException();
        }
    }
}