using System;
using System.Xml.Serialization;

namespace SocketServer.Shared.Event
{
    public class RoomUserUpdateEvent //: IEvent
    {
        [XmlElement]
        public string EventName
        {
            get { return "RoomUserUpdateEvent"; }
        }

        [XmlElement]
        public long RoomId { get; set; }

        [XmlElement]
        public long ZoneId { get; set; }

        [XmlElement]
        public RoomUserUpdateAction Action { get; set; }

        [XmlElement]
        public string UserName { get; set; }

        [XmlElement]
        public string RoomName { get; set; }
    }

    public enum RoomUserUpdateAction
    {
        [XmlEnum]
        AddUser,
        [XmlEnum]
        DeleteUser
    }
}
