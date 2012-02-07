using System;
using System.Xml.Serialization;

namespace SocketServer.Shared.Response
{
    public class LoginResponse
    {
        [XmlAttribute]
        public bool Success { get; set; }

        [XmlAttribute]
        public string UserName { get; set; }
    }
}
