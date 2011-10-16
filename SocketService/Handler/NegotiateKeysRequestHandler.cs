using System;
using SocketService.Command;
using SocketService.Framework.Client.Request;
using SocketService.Framework.Messaging;
using SocketService.Framework.ServiceHandlerLib;

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