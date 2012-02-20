using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketServer.Shared.Messaging;
using System.IO;

namespace SocketServer.Messages
{
    public class MessageOne : IValidatedMessage
    {
        public int Value { get; set; }

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
