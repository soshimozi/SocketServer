namespace SocketServer.Core.Data
{
    public partial class User 
    {
        public User()
        {
            Id = AutoIdElement.GetNextID();
        }

    }
}
