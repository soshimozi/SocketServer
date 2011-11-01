using SocketService.Framework.Data;

namespace SocketService.Core.Data
{
    public partial class Zone
    {
        public Zone()
        {
            Id = AutoIdElement.GetNextID();
        }
    }
}
