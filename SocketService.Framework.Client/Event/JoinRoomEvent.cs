using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.SharedObjects;

namespace SocketService.Framework.Client.Event
{
    [Serializable]
    public class JoinRoomEvent : IEvent
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public string RoomDescription { get; set; }
        public bool Protected { get; set; }
        public int Capacity { get; set; }
        public bool Hidden { get; set; }
    
        public string EventName
        {
            get { return "JoinRoomEvent"; }
        }

        public List<RoomVariable> RoomVariables { get; set; }

        public List<UserListEntry> Users { get; set; }
    }
}
