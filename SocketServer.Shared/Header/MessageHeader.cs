using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SocketServer.Shared.Header
{
    public class MessageHeader
    {
        [XmlAttribute]
        public CompressionTypes CompressionType
        {
            get;
            set;
        }

        [XmlElement]
        public EncryptionHeader EncryptionHeader
        {
            get;
            set;
        }

    }
}
