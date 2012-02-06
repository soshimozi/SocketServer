using System;
using SocketServer.Command;
using SocketServer.Core.Messaging;
using SocketServer.Shared.Request;

namespace SocketServer.Handler
{
    public class LoginRequestHandler : IRequestHandler<LoginRequest>
    {
        public void HandleRequest(LoginRequest request, Guid connectionId)
        {
            if (request != null)
            {
                MSMQQueueWrapper.QueueCommand(
                    new LoginUserCommand(connectionId, request.LoginName)
                );

            }

        }
    }
}
