using System;
using SocketServer.Command;
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

        public void HandleRequest(LoginRequest request, Shared.Network.ClientConnection connection)
        {
            throw new NotImplementedException();
        }
    }
}
