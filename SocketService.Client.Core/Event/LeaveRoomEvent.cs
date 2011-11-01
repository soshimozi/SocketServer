using System;

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
