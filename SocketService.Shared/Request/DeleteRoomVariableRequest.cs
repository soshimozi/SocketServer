using System;

namespace SocketServer.Shared.Request
{
    [Serializable]
    public class DeleteRoomVariableRequest
    {
        public int ZoneId { get; set; }


        public int RoomId { get; set; }

        public string Name { get; set; }
    }
}