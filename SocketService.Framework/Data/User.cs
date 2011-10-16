namespace SocketService.Framework.Data
{
    public partial class User 
    {
        public User()
        {
            Id = AutoIdElement.GetNextID();
        }

    }
}
