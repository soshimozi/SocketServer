using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using SocketService.Framework.Messaging;
using SocketService.Framework.ServiceHandlerLib;
using SocketService.Framework.Client.Request;

namespace SocketService.Handlers
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
                //LoginResponse response = LoginUser(server, loginRequest.ClientId, loginRequest.RequestId, loginRequest.UserName);
                string loginName = request.LoginName;

                // queue up a new server message
                MSMQQueueWrapper.QueueCommand(
                    new LoginUserCommand(connectionId, loginName)
                );

                return true;

            }

            return false;
        }
    }
}
