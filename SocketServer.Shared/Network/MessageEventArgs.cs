using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketServer.Net.Client;
using SocketServer.Shared.Messaging;
using Google.ProtocolBuffers;

namespace SocketServer.Shared.Network
{
    public class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(ClientConnection source, IMessage message, byte[] raw)
        {
            ClientConnection = source;
            Message = message;            
            RawMessage = raw;
        }

        public ClientConnection ClientConnection { get; protected set; }
        public IMessage Message { get; protected set; }
        public byte[] RawMessage { get; protected set; }
    }
}
