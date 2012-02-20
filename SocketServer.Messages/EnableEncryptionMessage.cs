using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketServer.Shared.Messaging;

namespace SocketServer.Messages
{
    public class EnableEncryptionMessage : IValidatedMessage
    {
        public string MessageID
        {
            get;
            set;
        }

        public void Validate()
        {
        }

        public bool Enable
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
