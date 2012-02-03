using System;

namespace SocketServer.Event
{
    [Serializable]
    public class PublicMessageEvent : IEvent
    {
        public string EventName
        {
            get { return "PublicMessageEvent"; }
        }

        public int ZoneId
        {
            get;
            set;
        }

        public int RoomId
        {
            get;
            set;
        }

        public string UserName
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }
    
    }
}
