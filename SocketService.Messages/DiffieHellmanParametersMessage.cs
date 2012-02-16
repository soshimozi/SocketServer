using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketServer.Shared.Messaging;
using ProtoBuf;

namespace SocketServer.Messages
{
    [ProtoContract]
    public class DiffieHellmanInfo
    {
        [ProtoMember(1)]
        public string P
        {
            get;
            set;
        }

        [ProtoMember(2)]
        public string G
        {
            get;
            set;
        }

        [ProtoMember(3)]
        public byte[] PublicKeyInfo
        {
            get;
            set;
        }
    }
}
