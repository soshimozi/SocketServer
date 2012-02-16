using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SocketServer
{
    public class PlainEnvelope : Envelope
    {
        public override void Serialize(IMessage message, Stream stream)
        {
            Message msg = message as Message;
            StringBuilder str = new StringBuilder();
            str.Append(msg.Name);
            if (!string.IsNullOrEmpty(msg.DefaultParameter))
                str.Append(" " + msg.DefaultParameter);

            str.Append("\r\n");
            byte[] nameBytes = MessageEncoding.GetBytes(str.ToString());
            stream.Write(nameBytes, 0, nameBytes.Length);
            message.Serialize(stream, MessageEncoding);
            stream.Flush();
        }

        public override IMessage Deserialize(Stream stream)
        {
            // The command should be on its own line
            string commandLine = ProtocolUtils.ReadLine(stream, MessageEncoding);
            while (commandLine.Length == 0)
                commandLine = ProtocolUtils.ReadLine(stream, MessageEncoding);

            string[] commandTokens = commandLine.Split(" ".ToCharArray());
            IMessage message = MessageFactory.Create(commandTokens[0]);

            if (message != null)
            {
                if (commandTokens.Length >= 2)
                {
                    Message msg = message as Message;
                    msg.DefaultParameter = commandTokens[1];
                }
                message.Deserialize(stream, MessageEncoding);
            }
            else
            {
            //    NullMessage nullMsg = new NullMessage();
            //    nullMsg.Deserialize(stream, MessageEncoding);
                throw new ProtocolException("Unknown message: " + commandLine);
            }

            return message;
        }
    }
}
