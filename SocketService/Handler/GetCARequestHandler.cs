using System;
using SocketService.Command;
using SocketService.Core.Messaging;
using SocketService.Core.ServiceHandlerLib;
using SocketService.Shared.Request;

namespace SocketService.Handler
{
    [Serializable]
    [ServiceHandlerType(typeof (GetCentralAuthorityRequest))]
    public class GetCARequestHandler :
        BaseHandler<GetCentralAuthorityRequest, Guid>
    {
        public override bool HandleRequest(GetCentralAuthorityRequest request, Guid state)
        {
            MSMQQueueWrapper.QueueCommand(
                new GetCentralAuthorityCommand(state)
                );

            return true;
        }
    }
}