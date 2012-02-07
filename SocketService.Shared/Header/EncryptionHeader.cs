using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SocketServer.Shared.Header
{
    public class EncryptionHeader
    {
        [XmlAttribute]
        public EncryptionTypes EncryptionType
        {
            get;
            set;
        }

        [XmlElement(IsNullable=true)]
        public byte [] PublicKey
        {
            get;
            set;
        }

    }
}
