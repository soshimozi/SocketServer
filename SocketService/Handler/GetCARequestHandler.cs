using System;
using SocketServer.Command;
using SocketServer.Core.Messaging;
using SocketServer.Core.ServiceHandlerLib;
using SocketServer.Shared.Request;

namespace SocketServer.Handler
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