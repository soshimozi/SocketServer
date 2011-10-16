namespace SocketService.Client.API.Data
{
    public class User
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


        public Room Room { get; set; }
    }
}
