using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace SocketServer.Messaging
{
    [ProtoContract]
    public class DHKeys
    {
        [ProtoMember(1, IsRequired = true)]
        public string P { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public string G { get; set; }
    }
}
