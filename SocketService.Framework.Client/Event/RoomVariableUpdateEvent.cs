using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.SharedObjects;

namespace SocketService.Framework.Client.Event
{
    [Serializable]
    public class RoomVariableUpdateEvent : IEvent
    {
        public string EventName
        {
            get { return "RoomVariableUpdateEvent"; }
        }

        public int RoomId { get; set; }
        public string Name { get; set; }
        public RoomVariable Variable { get; set; }
        public RoomVariableUpdateAction Action { get; set; }

        public int ZoneId { get; set; }
    }

    public enum RoomVariableUpdateAction
    {
        Add,
        Delete,
        Update
    }
}
