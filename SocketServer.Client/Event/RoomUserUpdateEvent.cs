using System;

namespace SocketService.Framework.Client.Event
{
    [Serializable]
    public class RoomUserUpdateEvent : IEvent
    {
        public string EventName
        {
            get { return "RoomUserUpdateEvent"; }
        }

        public long RoomId { get; set; }
        public long ZoneId { get; set; }
        public RoomUserUpdateAction Action { get; set; }
        public string UserName { get; set; }

        public string RoomName { get; set; }
    }

    public enum RoomUserUpdateAction
    {
        AddUser,
        DeleteUser
    }
}
