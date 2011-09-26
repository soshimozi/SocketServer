using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.Client.Event;

namespace SocketService.Client.API
{
    public class RoomVariableUpdateArgs : EventArgs
    {
        public RoomVariableUpdateEvent Event { get; set; }
    }
}
