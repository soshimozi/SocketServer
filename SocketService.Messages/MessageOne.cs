using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using SocketServer.Shared.Messaging;
using System.IO;

namespace SocketServer.Messages
{
    [ProtoContract]
    public class MessageOne : IMessage
    {
        [ProtoMember(1)]
        public int Value { get; set; }

        [ProtoMember(2)]
        public string MessageID
        {
            get; set;
        }

        public void Validate()
        {
        }

        public void Deserialize(Stream stream, Encoding encoding)
        {
        }

        public void Serialize(Stream stream, Encoding encoding)
        {
        }
    }
}
