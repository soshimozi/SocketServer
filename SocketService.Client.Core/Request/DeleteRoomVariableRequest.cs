using System;

namespace SocketService.Client.Core.Request
{
    [Serializable]
    public class DeleteRoomVariableRequest
    {
        public int ZoneId { get; set; }


        public int RoomId { get; set; }

        public string Name { get; set; }
    }
}