using System;
using SocketServer.Shared.Request;

namespace SocketServer.Handler
{
    public class GetRoomVariableRequestHandler : IRequestHandler<GetRoomVariableRequest>
    {
        public void HandleRequest(GetRoomVariableRequest request, Guid state)
        {
            if (request != null)
            {
                //string roomName = request.RoomName;

                //MSMQQueueWrapper.QueueCommand(
                //    new GetRoomVariableCommand(state, request.RoomName, request.VariableName)
                //);

            }
        }

        public void HandleRequest(GetRoomVariableRequest request, Shared.Network.ClientConnection connection)
        {
            throw new NotImplementedException();
        }
    }
}