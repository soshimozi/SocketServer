using SocketService.Framework.Data;

namespace SocketService.Core.Data
{
    public partial class User 
    {
        public User()
        {
            Id = AutoIdElement.GetNextID();
        }

    }
}
