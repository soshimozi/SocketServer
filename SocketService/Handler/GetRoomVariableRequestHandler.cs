using System;
using SocketService.Core.ServiceHandlerLib;
using SocketService.Shared.Request;

namespace SocketService.Handler
{
    [Serializable]
    [ServiceHandlerType(typeof (GetRoomVariableRequest))]
    public class GetRoomVariableRequestHandler : BaseHandler<GetRoomVariableRequest, Guid>
    {
        public override bool HandleRequest(GetRoomVariableRequest request, Guid state)
        {
            if (request != null)
            {
                //string roomName = request.RoomName;

                //MSMQQueueWrapper.QueueCommand(
                //    new GetRoomVariableCommand(state, request.RoomName, request.VariableName)
                //);

                return true;
            }

            return false;
        }
    }
}