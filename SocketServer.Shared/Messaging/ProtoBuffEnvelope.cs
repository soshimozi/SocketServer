using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Google.ProtocolBuffers;
using Google.ProtocolBuffers.DescriptorProtos;
using Google.ProtocolBuffers.Descriptors;
using log4net;
using SocketServer.Shared.Reflection;
using SocketServer.Shared.Serialization;
using System.Reflection;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using SocketServer.Crypto;
using log4net.Core;

namespace SocketServer.Shared.Messaging
{
    public class ProtoBuffEnvelope : MessageEnvelope
    {
        private static ILog logger = LogManager.GetLogger(typeof(ProtoBuffEnvelope));

        private readonly MessageRegistry registry;

        private bool encryptionEnabled = false;
        private AsymmetricKeyParameter remotePublicKey = null;
        private ServerAuthority serverAuthority = null;
        
        public ProtoBuffEnvelope(MessageRegistry messageRegistry)
        {
            this.registry = messageRegistry;
        }

        public void DisableEncryption()
        {
            encryptionEnabled = false;
            this.serverAuthority = null;
            this.remotePublicKey = null;
        }

        public void EnableEncryption(ServerAuthority serverAuthority, byte[] encodedPublicKey)
        {
            encryptionEnabled = true;
            this.serverAuthority = serverAuthority;
            this.remotePublicKey = new DHPublicKeyParameters(
                ((DHPublicKeyParameters)PublicKeyFactory.CreateKey(encodedPublicKey)).Y, serverAuthority.Parameters);

        }

        public override void Serialize(object messageObject, Stream stream)
        {
            IMessage message = (IMessage)messageObject;
            string descriptorName = message.DescriptorForType.FullName;

            logger.Logger.Log(typeof(ProtoBuffEnvelope), Level.Finer, string.Format("sending {0}", descriptorName), null);

            if (encryptionEnabled)
            {
                string privateKey = serverAuthority.GenerateAgreementValue(remotePublicKey).ToString(16);

                RijndaelCrypto crypto = new RijndaelCrypto();
                string encryptedName = crypto.Encrypt(descriptorName, privateKey);

                stream.Write(encryptedName.SerializeUTF());

                //StringBuilder str = new StringBuilder();
                //str.AppendFormat("{0}\r\n", encryptedName);

                //byte[] nameBytes = MessageEncoding.GetBytes(str.ToString());
                //stream.Write(nameBytes, 0, nameBytes.Length);

                byte[] messageBytes = message.ToByteArray();
                byte[] encryptedBytes = crypto.Encrypt(messageBytes, privateKey);
                stream.Write(encryptedBytes.Serialize());
            }
            else
            {
                //StringBuilder str = new StringBuilder();
                //str.AppendFormat("{0}\r\n", descriptorName);

                stream.Write(descriptorName.SerializeUTF());

                //byte[] nameBytes = MessageEncoding.GetBytes(str.ToString());
                //stream.Write(nameBytes, 0, nameBytes.Length);

                byte[] messageBytes = message.ToByteArray();
                stream.Write(messageBytes.Serialize());
            }
        }

        public override object Deserialize(Stream stream)
        {
            if (encryptionEnabled)
            {
                string descriptorName = stream.ReadUTF();
                int length = stream.ReadInt();
                byte[] buffer = stream.Read(length);

                string privateKey = serverAuthority.GenerateAgreementValue(remotePublicKey).ToString(16);
                RijndaelCrypto crypto = new RijndaelCrypto();

                descriptorName = crypto.Decrypt(descriptorName, privateKey);

                logger.Logger.Log(typeof(ProtoBuffEnvelope), Level.Finer, string.Format("recieving {0}", descriptorName), null);

                Type t = registry.GetTypeForDescriptor(descriptorName);
                if (t != null)
                {
                    return ReflectionHelper.InvokeStaticMethodOnType(t, "ParseFrom", crypto.Decrypt(buffer, privateKey));
                }

            }
            else
            {
                string descriptorName = stream.ReadUTF();

                int length = stream.ReadInt();
                byte[] buffer = stream.Read(length);

                logger.Logger.Log(typeof(ProtoBuffEnvelope), Level.Finer, string.Format("recieving {0}", descriptorName), null);

                Type t = registry.GetTypeForDescriptor(descriptorName);
                if (t != null)
                {
                    return ReflectionHelper.InvokeStaticMethodOnType(t, "ParseFrom", buffer);
                }
            }

            return null;
        }
    }
}
