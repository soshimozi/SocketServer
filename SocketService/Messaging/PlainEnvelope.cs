using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SocketServer.Messaging;
using SocketServer.Serialization;
using ProtoBuf;
using SocketServer.Reflection;

namespace SocketServer.Messaging
{
    public class PlainEnvelope : MessageEnvelope
    {
        public override void Serialize(object message, Stream stream)
        {
            string typeName = message.GetType().AssemblyQualifiedName;

            StringBuilder str = new StringBuilder();
            str.AppendFormat("{0}\r\n", typeName);

            byte[] nameBytes = MessageEncoding.GetBytes(str.ToString());
            stream.Write(nameBytes, 0, nameBytes.Length);

            MemoryStream memoryStream = new MemoryStream();
            Serializer.NonGeneric.Serialize(memoryStream, message);

            byte[] data = memoryStream.ToArray();
            stream.Write(data.Length.Serialize());
            stream.Write(data);
            stream.Flush();
        }

        public override object Deserialize(Stream stream)
        {
            string typeName = stream.ReadLine(MessageEncoding);

            int length = stream.ReadInt();
            byte[] messageBuffer = stream.Read(length);
            Type type = ReflectionHelper.FindType(typeName);

            if (type != null)
            {
                MemoryStream messageStream = new MemoryStream(messageBuffer);
                return Serializer.NonGeneric.Deserialize(type, messageStream);
            }
            else
            {
                throw new Exception("Invalid message type");
            }
        }

    }

}
