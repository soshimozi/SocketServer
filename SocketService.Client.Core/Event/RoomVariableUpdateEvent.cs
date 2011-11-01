using System;
using SocketService.Framework.Client.SharedObjects;

namespace SocketService.Framework.Client.Event
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
        public SharedObject Value { get; set; }
        public RoomVariableUpdateAction Action { get; set; }
    }

    public enum RoomVariableUpdateAction
    {
        Add,
        Delete,
        Update
    }
}
