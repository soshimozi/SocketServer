using System;
using SocketService.Command;
using SocketService.Core.Messaging;
using SocketService.Core.ServiceHandlerLib;
using SocketService.Shared.Request;

namespace SocketService.Handler
{
    [Serializable]
    [ServiceHandlerType(typeof (NegotiateKeysRequest))]
    public class NegotiateKeysRequestHandler :
        BaseHandler<NegotiateKeysRequest, Guid>
    {
        public override bool HandleRequest(NegotiateKeysRequest request, Guid state)
        {
            MSMQQueueWrapper.QueueCommand(
                new NegotiateKeysCommand(state, request.PublicKey)
                );

            return true;
        }
    }
}