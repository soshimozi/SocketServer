using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Framework.Client.Event
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
