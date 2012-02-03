using System;
using SocketServer.Command;
using SocketServer.Core.Messaging;
using SocketServer.Core.ServiceHandlerLib;
using SocketServer.Shared.Request;

namespace SocketServer.Handler
{
    [Serializable]
    [ServiceHandlerType(typeof (CreateRoomRequest))]
    internal class ChangeRoomRequestHandler : BaseHandler<CreateRoomRequest, Guid>
    {
        public override bool HandleRequest(CreateRoomRequest request, Guid state)
        {
            if (request != null)
            {
                string roomName = request.RoomName;

                MSMQQueueWrapper.QueueCommand(
                    new CreateRoomCommand(state, request.ZoneName, roomName)
                    );

                return true;
            }

            return false;
        }
    }
}