using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SocketServer
{
    public interface IMessage
    {
        string MessageID { get; set; }
        void Serialize(Stream stream, Encoding encoding);
        void Deserialize(Stream stream, Encoding encoding);
        void Validate();
        string Name { get; }
    }
}
