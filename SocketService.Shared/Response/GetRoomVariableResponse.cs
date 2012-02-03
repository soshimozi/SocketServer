namespace SocketServer.Shared.Response
{
    public class GetRoomVariableResponse : IServerResponse
    {
        public long ZoneId { get; set; }

        public long RoomId { get; set; }

        public string Name { get; set; }

        public object Value { get; set; }
    }
}
