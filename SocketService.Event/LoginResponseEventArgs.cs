using System;
using SocketServer.Shared.Response;

namespace SocketServer.Event
{
    public class LoginResponseEventArgs : EventArgs
    {
        public LoginResponse Response { get; set; }

    }
}
