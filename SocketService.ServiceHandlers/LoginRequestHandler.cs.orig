using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceHandlerLib;
using System.ComponentModel.Composition;
using BlazeRequestResponse;
using BlazeServerData;

namespace ServiceHandlers
{
    [Export(typeof(IServiceHandler))]
    [ServiceHandlerType(typeof(LoginRequest))]
    public class LoginRequestHandler : IServiceHandler
    {
        #region IServiceHandler Members

        private UserManager _userManager;

        public LoginRequestHandler()
        {
            _userManager = new UserManager(DataEntityFactory.GetDataEntities());
        }

        public bool HandleRequest(IRequest request, IServerContext server)
        {
            LoginRequest loginRequest = request as LoginRequest;

            if (loginRequest != null)
            {
                LoginResponse response = LoginUser(server, loginRequest.ClientId, loginRequest.RequestId, loginRequest.UserName);
                server.SendObject(response, loginRequest.ClientId);
                return true;
            }

            return false;
        }

        private static int _lastCount = 0;
        private LoginResponse LoginUser(IServerContext server, Guid clientId, string requestId, string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                userName = string.Format("user{0:00}", _lastCount++);
            }

            User user = _userManager.FindUser(userName);
            if (user == null)
            {
                user = _userManager.CreateUser();
                user.UserName = userName;
            }

            LoginResponse response;
            //if (user.IsActive)
            //{
            //    // TODO: Handle duplicate login
            //    response = new LoginResponse(false, userName, requestId);
            //    return response;
            //}

            user.IsActive = true;
            user.Room = null;

            _userManager.SaveChanges();
            server.AssociateClientWithUser(clientId, user.UserId);

            response = new LoginResponse(true, userName, requestId);
            return response;
        }

        #endregion
    }
}
