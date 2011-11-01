using System;
using SocketService.Shared.Response;

namespace SocketService.Event
{
    public class LoginResponseEventArgs : EventArgs
    {
        public LoginResponse Response { get; set; }

    }
}
