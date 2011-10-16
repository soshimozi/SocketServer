using System;
using SocketService.Framework.Client.Request;
using SocketService.Framework.ServiceHandlerLib;

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