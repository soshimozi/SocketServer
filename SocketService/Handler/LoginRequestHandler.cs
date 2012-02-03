using System;
using SocketServer.Command;
using SocketServer.Core.Messaging;
using SocketServer.Core.ServiceHandlerLib;
using SocketServer.Shared.Request;

namespace SocketServer.Handler
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
