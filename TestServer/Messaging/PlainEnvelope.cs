using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TestServer.Shared.Interop.Java;

namespace TestServer.Messaging
{
    public class PlainEnvelope : Envelope
    {
        public override void Serialize(IMessage message, Stream stream)
        {
            PlainMessage msg = message as PlainMessage;
            StringBuilder str = new StringBuilder();
            str.Append(msg.Name);
            str.Append("\r\n");
            byte[] nameBytes = MessageEncoding.GetBytes(str.ToString());
            stream.Write(nameBytes, 0, nameBytes.Length);
            message.Serialize(stream, MessageEncoding);
            stream.Flush();
        }

        public override IMessage Deserialize(Stream stream)
        {
            // The command should be on its own line
            string messageName = stream.ReadLine(MessageEncoding);
            //while (commandLine.Length == 0)
            //    commandLine = stream.ReadLine(MessageEncoding);

            //string[] commandTokens = commandLine.Split(" ".ToCharArray());

            IMessage message = MessageFactory.Create(messageName);
            if (message != null)
            {
                message.Deserialize(stream, MessageEncoding);
            }
            else
            {
                throw new Exception("Unknown message: " + messageName);
            }

            return message;
        }
    }

}
