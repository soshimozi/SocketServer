using System;
using SocketService.Command;
using SocketService.Framework.Client.Request;
using SocketService.Framework.Messaging;
using SocketService.Framework.ServiceHandlerLib;

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
