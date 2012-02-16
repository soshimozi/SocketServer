using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketServer.Shared.Messaging;
using ProtoBuf;

namespace SocketServer.Messages
{
    [ProtoContract]
    public class ServerConnectionResponse : IMessage
    {
        [ProtoMember(1)]
        public string MessageID
        {
            get; set;
        }

        [ProtoMember(2)]
        public DiffieHellmanInfo DiffieHellmanInfo
        {
            get;
            set;
        }

        public void Validate()
        {
        }
    }
}
