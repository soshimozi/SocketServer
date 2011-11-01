using System;

namespace SocketService.Shared.Response
{
    [Serializable]
    public class LoginResponse : IServerResponse
    {
        public bool Success { get; set; }
        public string UserName { get; set; }
    }
}
