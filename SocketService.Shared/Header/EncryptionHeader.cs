using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SocketServer.Shared.Header
{
    [Serializable]
    public class EncryptionHeader
    {
        [XmlAttribute]
        public EncryptionTypes EncryptionType
        {
            get;
            set;
        }

        [XmlAttribute]
        public string PublicKey
        {
            get;
            set;
        }

    }
}
