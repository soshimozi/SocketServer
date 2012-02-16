using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketServer.Shared.Messaging;
using ProtoBuf;

namespace SocketServer.Messages
{
    [ProtoContract]
    public class EnableEncryptionMessage : IMessage
    {
        [ProtoMember(1)]
        public string MessageID
        {
            get;
            set;
        }

        public void Validate()
        {
        }

        [ProtoMember(2)]
        public bool Enable
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
