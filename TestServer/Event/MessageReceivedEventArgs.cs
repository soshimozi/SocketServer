using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestServer.Network;
using TestServer.Messaging;

namespace TestServer.Event
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageReceivedEventArgs(ClientConnection source, IMessage message, byte[] raw)
        {
            ClientConnection = source;
            Message = message;
            RawMessage = raw;
            IsError = false;
            ErrorMessage = string.Empty;
        }

        public ClientConnection ClientConnection { get; protected set; }
        public IMessage Message { get; protected set; }
        public byte[] RawMessage { get; protected set; }
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
    }
}
