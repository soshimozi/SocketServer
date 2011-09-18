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
    [ServiceHandlerType(typeof(GetCentralAuthorityRequest))]
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
