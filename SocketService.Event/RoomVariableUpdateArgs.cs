using System;

namespace SocketService.Event
{
    public class RoomVariableUpdateArgs : EventArgs
    {
        public RoomVariableUpdateEvent Event { get; set; }
    }
}
