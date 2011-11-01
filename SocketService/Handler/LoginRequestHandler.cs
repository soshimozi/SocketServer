using System;
using SocketService.Command;
using SocketService.Core.Messaging;
using SocketService.Core.ServiceHandlerLib;
using SocketService.Shared.Request;

namespace SocketService.Handler
{
    [Serializable]
    [ServiceHandlerType(typeof(LoginRequest))]
    public class LoginRequestHandler : BaseHandler<LoginRequest, Guid>
    {
        public override bool HandleRequest(LoginRequest request, Guid connectionId)
        {
            if (request != null)
            {
                MSMQQueueWrapper.QueueCommand(
                    new LoginUserCommand(connectionId, request.LoginName)
                );

                return true;

            }

            return false;
        }
    }
}
