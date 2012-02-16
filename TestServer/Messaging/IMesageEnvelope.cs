using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TestServer.Messaging
{
    public interface IMesageEnvelope
    {
        void Serialize(IMessage message, Stream stream);
        IMessage Deserialize(Stream stream);
    }
}
