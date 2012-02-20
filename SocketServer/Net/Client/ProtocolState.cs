using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketServer.Net.Client
{
    public enum ProtocolState
    {
        WaitingForHeader,
        WaitingForBody
    }
}
