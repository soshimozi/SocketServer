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
    [ServiceHandlerType(typeof(LoginRequest))]
    public class LoginRequestHandler : BaseHandler<LoginRequest, Guid>
    {
        public LoginRequestHandler()
        {
        }

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
