using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using SocketService.Request;
using SocketService.Crypto;
using SocketService.Command;
using SocketService.ServiceHandler;
using SocketService.Actions;
using SocketService.Messaging;

namespace SocketService.Handler
{
    [Serializable()]
    [ServiceHandlerType(typeof(LoginRequest))]
    [Export(typeof(IServiceHandler))]
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

        //private static int _lastCount = 0;
        //private LoginResponse LoginUser(IServerContext server, Guid clientId, string requestId, string userName)
        //{
        //    if (string.IsNullOrEmpty(userName))
        //    {
        //        userName = string.Format("user{0:00}", _lastCount++);
        //    }

        //    User user = _userManager.FindUser(userName);
        //    if (user == null)
        //    {
        //        user = _userManager.CreateUser();
        //        user.UserName = userName;
        //    }

        //    LoginResponse response;
        //    //if (user.IsActive)
        //    //{
        //    //    // TODO: Handle duplicate login
        //    //    response = new LoginResponse(false, userName, requestId);
        //    //    return response;
        //    //}

        //    user.IsActive = true;
        //    user.Room = null;

        //    _userManager.SaveChanges();
        //    server.AssociateClientWithUser(clientId, user.UserId);

        //    response = new LoginResponse(true, userName, requestId);
        //    return response;
        //}
    }
}
