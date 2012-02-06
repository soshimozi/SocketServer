using System;

namespace SocketServer.Shared.Response
{
    [Serializable]
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string UserName { get; set; }
    }
}
