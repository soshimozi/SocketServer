using System;
using SocketService.Command;
using SocketService.Framework.Client.Request;
using SocketService.Framework.Messaging;
using SocketService.Framework.ServiceHandlerLib;

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