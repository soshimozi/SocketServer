using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketServer.Shared.Request;
using SocketServer.Core.ServiceHandlerLib;
using SocketServer.Core.Messaging;
using SocketServer.Command;

namespace SocketServer.Handler
{
    [Serializable]
    [ServiceHandlerType(typeof(GetKeyParametersRequest))]
    public class GetKeyParametersRequestHandler : BaseHandler<GetKeyParametersRequest, Guid>
    {
        public override bool HandleRequest(GetKeyParametersRequest request, Guid state)
        {
            if (request != null)
            {
                MSMQQueueWrapper.QueueCommand(
                    new GetKeyParametersCommand(state)
                );

                return true;

            }

            return false;

        }
    }
}
