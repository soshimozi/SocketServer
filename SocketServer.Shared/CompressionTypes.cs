using System.Xml.Serialization;

namespace SocketServer.Shared
{
    public enum CompressionTypes
    {
        [XmlEnum]
        GZip,
        [XmlEnum]
        None
    }
}