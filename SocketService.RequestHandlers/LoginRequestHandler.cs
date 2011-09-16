using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.ServiceHandler;
using SocketService.Crypto;
using SocketService.Request;
using SocketLib;
using System.ComponentModel.Composition;

namespace SocketService.Handlers
{
    [Export(typeof(IServiceHandler))]
    [ServiceHandlerType(typeof(LoginRequest))]
    [Serializable()]
    public class LoginRequestHandler : IServiceHandler
    {
        public bool HandleRequest(object request, object state)
        {
            LoginRequest loginRequest = request as LoginRequest;
            Guid clientId = (Guid)state;

            if (loginRequest != null)
            {
                return true;
            }

            return false;
        }
    }
}
