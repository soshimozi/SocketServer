using System;

namespace SocketService.Event
{
    public class GetRoomVariableResponseArgs : EventArgs
    {
        public long ZoneId { get; set; }

        public long RoomId { get; set; }

        public string Name { get; set; }

        public object Value { get; set; }
    }
}
