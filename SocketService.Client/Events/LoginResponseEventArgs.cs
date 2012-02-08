using System;
using SocketServer.Shared.Response;

namespace SocketServer.Client
{
    public class LoginResponseEventArgs : EventArgs
    {
        public LoginResponse Response { get; set; }

    }
}
