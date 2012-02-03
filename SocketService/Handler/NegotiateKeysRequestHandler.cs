using System;
using SocketServer.Command;
using SocketServer.Core.Messaging;
using SocketServer.Core.ServiceHandlerLib;
using SocketServer.Shared.Request;

namespace SocketServer.Handler
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