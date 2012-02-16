using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SocketServer
{
    public interface IEnvelope
    {
        Encoding MessageEncoding { get; set; }

        string MessageEncodingString { get; set; }

        void Serialize(IMessage message, Stream stream);
        IMessage Deserialize(Stream stream);

    }
}
