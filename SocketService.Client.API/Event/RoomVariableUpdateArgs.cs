using System;
using SocketService.Framework.Client.Event;

namespace SocketService.Client.API.Event
{
    public class RoomVariableUpdateArgs : EventArgs
    {
        public RoomVariableUpdateEvent Event { get; set; }
    }
}
