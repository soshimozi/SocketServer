namespace SocketServer.Data
{
    public partial class Room
    {
        public Room()
        {
            Id = AutoIdElement.GetNextID();
        }
    }
}
