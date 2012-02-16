using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace SocketServer.Messaging
{
    [ProtoContract]
    public class ConnectionResponse
    {
        [ProtoMember(1, IsRequired = true)]
        public byte[] ServerPublicKey { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public DHKeys DHKeys { get; set; }

    }
}
