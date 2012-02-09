using System;
using SocketServer.Shared.Event;

namespace SocketServer.Client
{
    public class RoomVariableUpdateArgs : EventArgs
    {
        public RoomVariableUpdateEvent Event { get; set; }
    }
}
