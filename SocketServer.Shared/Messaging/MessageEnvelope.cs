using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SocketServer.Shared.Messaging
{
    public abstract class MessageEnvelope
    {
        protected MessageEnvelope()
        {
            MessageEncoding = Encoding.ASCII;
        }

        public Encoding MessageEncoding { get; set; }

        public abstract void Serialize(object message, Stream stream);
        public abstract object Deserialize(Stream stream);
    }
}
