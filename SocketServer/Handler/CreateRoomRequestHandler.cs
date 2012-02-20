using System;
using SocketServer.Command;
using SocketServer.Shared.Request;

namespace SocketServer.Handler
{
    public class CreateRoomRequestHandler : IRequestHandler<CreateRoomRequest>
    {
        public void HandleRequest(CreateRoomRequest request, Guid state)
        {
            if (request != null)
            {
                string roomName = request.RoomName;

                MSMQQueueWrapper.QueueCommand(
                    new CreateRoomCommand(state, request.ZoneName, roomName)
                    );

            }

        }

        public void HandleRequest(CreateRoomRequest request, Shared.Network.ClientConnection connection)
        {
            throw new NotImplementedException();
        }
    }
}