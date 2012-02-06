using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SocketServer.Shared
{
    public enum ResponseTypes
    {
        [XmlEnum]
        NegotiateKeysResponse
    }
}
