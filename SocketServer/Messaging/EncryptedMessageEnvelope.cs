using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketServer.Crypto;
using System.IO;
using ProtoBuf;
using SocketServer.Reflection;
using SocketServer.Serialization;

namespace SocketServer.Messaging
{
    class EncryptedMessageEnvelope : MessageEnvelope
    {
        public EncryptedMessageEnvelope()
        {
            MessageEncoding = ASCIIEncoding.UTF8;
        }

        public EncryptedMessageEnvelope(ICrypto crypto)
        {
            MessageEncoding = ASCIIEncoding.UTF8;
            Crypto = crypto;
        }

        public EncryptedMessageEnvelope(Encoding encoding, ICrypto crypto)
        {
            MessageEncoding = encoding;
            Crypto = crypto;
        }

        public EncryptedMessageEnvelope(Encoding encoding)
        {
            MessageEncoding = encoding;
        }

        public ICrypto Crypto
        { get; set; }

        public override void Serialize(object message, Stream stream)
        {
            string typeName = message.GetType().FullName;

            StringBuilder str = new StringBuilder();
            str.AppendFormat("{0}\r\n", Crypto.Encrypt(typeName));

            byte[] nameBytes = MessageEncoding.GetBytes(str.ToString());
            stream.Write(nameBytes, 0, nameBytes.Length);

            MemoryStream memoryStream = new MemoryStream();
            Serializer.NonGeneric.Serialize(memoryStream, message);

            byte[] encryptedData = Crypto.Encrypt(memoryStream.ToArray());
            stream.Write(encryptedData.Length.Serialize());
            stream.Write(encryptedData);
            stream.Flush();
        }

        public override object Deserialize(Stream stream)
        {
            string typeName = Crypto.Decrypt(stream.ReadLine(MessageEncoding));

            int length = stream.ReadInt();
            byte[] messageBuffer = stream.Read(length);
            Type type = ReflectionHelper.FindType(typeName);

            if (type != null)
            {
                MemoryStream messageStream = new MemoryStream(Crypto.Decrypt(messageBuffer));
                return Serializer.NonGeneric.Deserialize(type, messageStream);
            }
            else
            {
                throw new Exception("Invalid message type");
            }
        }

    }
}
