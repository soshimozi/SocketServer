using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketServer.Shared.Messaging;

namespace SocketServer.Messages
{
    public class EnableEncryptionResponse : IValidatedMessage
    {
        public string MessageID
        {
            get; set;
        }

        public void Validate()
        {
            throw new NotImplementedException();
        }

        public bool Enabled
        {
            get;
            set;
        }
    }
}
