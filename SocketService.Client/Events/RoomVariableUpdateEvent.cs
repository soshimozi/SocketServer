using System;

namespace SocketServer.Client
{
    [Serializable]
    public class RoomVariableUpdateEvent : IEvent
    {
        public string EventName
        {
            get { return "RoomVariableUpdateEvent"; }
        }

        public long ZoneId { get; set; }
        public long RoomId { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
        public RoomVariableUpdateAction Action { get; set; }
    }

    public enum RoomVariableUpdateAction
    {
        Add,
        Delete,
        Update
    }
}
