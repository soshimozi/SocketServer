namespace SocketServer.Data
{
    public partial class User 
    {
        public User()
        {
            Id = AutoIdElement.GetNextID();
        }

    }
}
