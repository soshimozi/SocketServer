using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.ServiceHandlerLib;
using SocketService.Framework.Request;
using SocketService.Framework.Messaging;
using SocketService.Command;

namespace SocketService.Handler
{
    [Serializable]
    [ServiceHandlerType(typeof(UpdateRoomVariableRequest))]
    public class UpdateRoomVariableRequestHandler : BaseHandler<UpdateRoomVariableRequest, Guid>
    {
        public override bool HandleRequest(UpdateRoomVariableRequest request, Guid state)
        {
            MSMQQueueWrapper.QueueCommand(
                new UpdateRoomVariableCommand(state, request.ZoneId, request.RoomId, request.Name, request.Value)
            );

            return true;
        }
    }
}
