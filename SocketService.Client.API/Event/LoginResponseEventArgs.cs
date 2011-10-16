using System;
using SocketService.Framework.Client.Response;

namespace SocketService.Client.API.Event
{
    public class LoginResponseEventArgs : EventArgs
    {
        public LoginResponse LoginResponse
        {
            get;
            set;
        }
    }
}
