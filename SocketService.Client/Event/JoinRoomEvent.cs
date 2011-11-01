using System;
using SocketService.Framework.Client.SharedObjects;

namespace SocketService.Framework.Client.Event
{
    [Serializable]
    public class JoinRoomEvent : IEvent
    {
        public long ZoneId { get; set; }
        public long RoomId { get; set; }
        public string RoomName { get; set; }
        public string RoomDescription { get; set; }
        public bool Protected { get; set; }
        public int Capacity { get; set; }
        public bool Hidden { get; set; }
    
        public string EventName
        {
            get { return "JoinRoomEvent"; }
        }

        public SharedObject [] RoomVariables { get; set; }

        public UserListEntry [] Users { get; set; }
    }
}
