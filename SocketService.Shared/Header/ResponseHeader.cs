using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SocketServer.Shared.Header
{
    public class ResponseHeader
    {
        [XmlAttribute]
        public ResponseTypes ResponseType
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
