using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.Client.Event;

namespace SocketService.Client.API
{
    public class JoinRoomEventArgs : EventArgs
    {
        public JoinRoomEvent Event
        {
            get;
            set;
        }
    }
}
