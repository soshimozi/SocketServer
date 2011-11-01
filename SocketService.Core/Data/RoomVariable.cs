using SocketService.Framework.Data;

namespace SocketService.Core.Data
{
    public partial class RoomVariable
    {
        public RoomVariable()
        {
            Id = AutoIdElement.GetNextID();
        }
    }
}
