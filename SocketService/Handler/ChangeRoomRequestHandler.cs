using System;
using SocketServer.Command;
using SocketServer.Messaging;
using SocketServer.Shared.Request;

namespace SocketServer.Handler
{
    public class ChangeRoomRequestHandler : IRequestHandler<CreateRoomRequest>
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
    }
}