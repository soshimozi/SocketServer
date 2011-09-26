using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Framework.Client.Event
{
    [Serializable]
    public class LeaveRoomEvent : IEvent
    {
        #region IEvent Members

        public string EventName
        {
            get { return "LeaveRoomEvent"; }
        }

        #endregion

        public int RoomId { get; set; }
        public string UserName { get; set; }
    }
}
