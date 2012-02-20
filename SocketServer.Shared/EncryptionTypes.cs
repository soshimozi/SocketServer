using System.Xml.Serialization;

namespace SocketServer.Shared
{
    public enum EncryptionTypes
    {
        [XmlEnum]
        Blowfish,
        [XmlEnum]
        Des,
        [XmlEnum]
        TripleDes,
        [XmlEnum]
        Rsa,
        [XmlEnum]
        Aes,
        [XmlEnum]
        None
    }
}