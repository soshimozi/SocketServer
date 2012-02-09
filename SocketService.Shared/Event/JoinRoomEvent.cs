using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace SocketServer.Shared.Event
{
    public class JoinRoomEvent 
    {
        public JoinRoomEvent()
        {
            Users = new List<string>();
        }

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

        [XmlElement]
        public List<string> Users { get; set; }

        [XmlElement]
        public string EventName
        {
            get { return "JoinRoomEvent"; }
        }

        //public object [] RoomVariables { get; set; }

    }
}
