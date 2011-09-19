using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Framework.Client.Event
{
    [Serializable]
    public class RoomUserUpdateEvent : IEvent
    {
        public string EventName
        {
            get { return "RoomUserUpdateEvent"; }
        }

        public RoomUserUpdateAction Action { get; set; }
        public string UserName { get; set; }
        public int RoomId { get; set; }
    }

    public enum RoomUserUpdateAction
    {
        AddUser,
        DeleteUser
    }
}
