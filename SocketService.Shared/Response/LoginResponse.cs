namespace SocketService.Shared.Response
{
    public class LoginResponse : IServerResponse
    {
        public bool Success { get; set; }
        public string UserName { get; set; }
    }
}
