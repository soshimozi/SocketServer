using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketServer.Shared.Messaging;

namespace SocketServer.Messages
{
    public class DiffieHellmanInfo
    {
        public string P
        {
            get;
            set;
        }

        public string G
        {
            get;
            set;
        }

        public byte[] PublicKeyInfo
        {
            get;
            set;
        }
    }
}
