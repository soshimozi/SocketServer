using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SocketServer.Shared.Response
{
    public class NegotiateKeysResponse
    {
        [XmlElement]
        public byte [] ServerPublicKey { get; set; }

        [XmlElement]
        public String Prime { get; set; }

        [XmlElement]
        public String G { get; set; }
    }
}
