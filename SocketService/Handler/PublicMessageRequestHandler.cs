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
    [ServiceHandlerType(typeof(PublicMessageRequest))]
    public class PublicMessageRequestHandler : BaseHandler<PublicMessageRequest, Guid>
    {
        public override bool HandleRequest(PublicMessageRequest request, Guid state)
        {
            MSMQQueueWrapper.QueueCommand(
                new PublicMessageCommand(request.Zone, request.Room, request.User, request.Message)
            );

            return true;
        }
    }
}
