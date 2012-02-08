using System;
using System.Xml.Serialization;

namespace SocketServer.Shared.Response
{
    public class JoinRoomEvent
    {
        [XmlElement]
        public long ZoneId { get; set; }

        [XmlElement]
        public long RoomId { get; set; }

        [XmlElement]
        public string RoomName { get; set; }

        [XmlElement]
        public string RoomDescription { get; set; }

        [XmlElement]
        public bool Protected { get; set; }

        [XmlElement]
        public int Capacity { get; set; }

        [XmlElement]
        public bool Hidden { get; set; }
    
        public string EventName
        {
            get { return "JoinRoomEvent"; }
        }

        //public object [] RoomVariables { get; set; }

        [XmlElement]
        public string[] Users { get; set; }
    }
}
