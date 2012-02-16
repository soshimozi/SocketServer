using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketServer.Shared.Messaging;
using ProtoBuf;

namespace SocketServer.Messages
{
    [ProtoContract]
    public class EnableEncryptionResponse : IMessage
    {
        [ProtoMember(1)]
        public string MessageID
        {
            get; set;
        }

        public void Validate()
        {
            throw new NotImplementedException();
        }

        [ProtoMember(2)]
        public bool Enabled
        {
            get;
            set;
        }
    }
}
