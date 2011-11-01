using SocketService.Framework.Data;

namespace SocketService.Core.Data
{
    public partial class Room
    {
        public Room()
        {
            Id = AutoIdElement.GetNextID();
        }
    }
}
