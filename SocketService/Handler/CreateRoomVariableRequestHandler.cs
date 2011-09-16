using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.ServiceHandler;
using SocketService.Request;
using System.ComponentModel.Composition;
using SocketService.Messaging;
using SocketService.Command;

namespace SocketService.Handler
{
    [Serializable()]
    [ServiceHandlerType(typeof(CreateRoomVariableRequest))]
    [Export(typeof(IServiceHandler))]
    public class CreateRoomVariableRequestHandler : BaseHandler<CreateRoomVariableRequest, Guid>
    {
        public override bool HandleRequest(CreateRoomVariableRequest request, Guid state)
        {
            MSMQQueueWrapper.QueueCommand(
                new CreateRoomVariableCommand(request.Room, request.VariableName, request.Value)
            );

            return true;
        }
    }
}
