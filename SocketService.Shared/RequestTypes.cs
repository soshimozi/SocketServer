using System.Xml.Serialization;

namespace SocketServer.Shared
{
    public enum RequestTypes
    {
        [XmlEnum]
        NegotiateKeysRequest,
        [XmlEnum]
        LoginRequest,
        [XmlEnum]
        LogoutRequest,
        CreateRoomRequest
    }
}