using System;

namespace SocketServer.Event
{
    public class RoomVariableUpdateArgs : EventArgs
    {
        public RoomVariableUpdateEvent Event { get; set; }
    }
}
