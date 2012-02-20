using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TestServer.Messaging
{
    public interface IMessage
    {
        string MessageID { get; set; }
        void Validate();
        string Name { get; }
        void Deserialize(Stream stream, Encoding encoding);
        void Serialize(Stream stream, Encoding encoding);
    }
}
