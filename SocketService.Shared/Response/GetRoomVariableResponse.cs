namespace SocketServer.Shared.Response
{
    public class GetRoomVariableResponse
    {
        public long ZoneId { get; set; }

        public long RoomId { get; set; }

        public string Name { get; set; }

        public object Value { get; set; }
    }
}
