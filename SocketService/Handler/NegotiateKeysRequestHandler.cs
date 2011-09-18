using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using SocketService.Command;
using SocketService.Framework.Messaging;
using SocketService.Framework.ServiceHandlerLib;
using SocketService.Framework.Request;

namespace SocketService
{
    [Serializable()]
    [ServiceHandlerType(typeof(NegotiateKeysRequest))]
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
