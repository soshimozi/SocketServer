using System;
using System.Linq;
using SocketServer.Command;
using com.BlazeServer.Messages.MessageProtos;
using SocketServer.Shared.Network;
using SocketServer.Repository;
using SocketServer.Data;
using SocketServer.Actions;

namespace SocketServer.Handler
{
    public class LoginRequestHandler : IRequestHandler<LoginRequest>
    {
        public void HandleRequest(LoginRequest request, ClientConnection connection)
        {
            var success = false;

            // check if user exists
            var user = UserRepository.Instance.Query(u => u.Name == request.UserName).FirstOrDefault();
            if (user == null)
            {
                user = new User() { Name = request.UserName };
                UserRepository.Instance.Add(user);

                user.ClientKey = connection.ClientId;

                success = true;
            }

            LoginResponse.Builder newResponse = LoginResponse.CreateBuilder();
            newResponse.SetSuccess(success);

            connection.Send(newResponse.Build());
        }
    }
}
