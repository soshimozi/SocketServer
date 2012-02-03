using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketServer.Core.ServiceHandlerLib;
using SocketServer.Shared.Request;
using SocketServer.Core.Messaging;
using SocketServer.Command;

namespace SocketServer.Handler
{
    [Serializable]
    [ServiceHandlerType(typeof(NegotiateKeyRequest))]
    class NegotiateKeyRequestHandler : BaseHandler<NegotiateKeyRequest, Guid>
    {
        public override bool HandleRequest(NegotiateKeyRequest request, Guid state)
        {
            if (request != null)
            {
                MSMQQueueWrapper.QueueCommand(
                    new NegotiateKeyCommand(state, request.RemotePublicKey)
                );

                return true;

            }

            return false;
        }
    }
}
