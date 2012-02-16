using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SocketServer.Shared.Messaging
{
    public interface IMessage
    {
        string MessageID { get; set; }
        void Validate();
    }
}
