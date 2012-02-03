namespace SocketServer.Client.Data
{
    public class ClientUser
    {
        public string UserName
        {
            get;
            set;
        }

        public bool IsMe
        {
            get;
            set;
        }


        public ClientRoom Room { get; set; }
    }
}
