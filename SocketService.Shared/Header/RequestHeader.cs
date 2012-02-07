using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SocketServer.Shared.Header
{
    public class RequestHeader
    {
        [XmlAttribute]
        public RequestTypes RequestType
        {
            get;
            set;
        }

        [XmlElement]
        public MessageHeader MessageHeader
        {
            get;
            set;
        }

    }
}
