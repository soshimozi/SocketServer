using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketServer.Shared.Messaging;

namespace SocketServer.Messages
{
    public class ServerConnectionResponse : IValidatedMessage
    {
        public string MessageID
        {
            get; set;
        }

        public DiffieHellmanInfo DiffieHellmanInfo
        {
            get;
            set;
        }

        public void Validate()
        {
        }
    }
}
