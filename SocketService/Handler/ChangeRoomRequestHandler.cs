using System;
using SocketService.Command;
using SocketService.Core.Messaging;
using SocketService.Core.ServiceHandlerLib;
using SocketService.Shared.Request;

namespace SocketService.Handler
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