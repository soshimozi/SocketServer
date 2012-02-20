using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.ProtocolBuffers.DescriptorProtos;
using Google.ProtocolBuffers.Descriptors;
using System.IO;
using log4net;
using SocketServer.Shared.Reflection;

namespace SocketServer.Shared.Messaging
{
    public class MessageRegistry
    {
        private static ILog logger = LogManager.GetLogger(typeof(MessageRegistry));
        private readonly Dictionary<string, Type> mapping = new Dictionary<string, Type>();

        public MessageRegistry(string descriptionFile)
        {
            FileDescriptorSet descriptorSet = 
                FileDescriptorSet.ParseFrom(new FileStream(descriptionFile, FileMode.Open));

            foreach (FileDescriptorProto fdp in descriptorSet.FileList)
            {

                FileDescriptor fd = null;
                try
                {
                    fd = FileDescriptor.BuildFrom(fdp, new FileDescriptor[] { });
                }
                catch (DescriptorValidationException e)
                {
                    // TODO Auto-generated catch block
                    logger.Error(e);
                }

                foreach (MessageDescriptor descriptor in fd.MessageTypes)
                {
                    String className = fdp.Options.JavaPackage + "."
                        + fdp.Options.JavaOuterClassname + "."
                        + descriptor.Name;

                    Type type = ReflectionHelper.FindTypeFullSearch(className);

                    if (type != null)
                    {
                        mapping.Add(descriptor.FullName, type);
                    }
                }
            }
        }

        public Type GetTypeForDescriptor(string descriptorName)
        {
            if (mapping.ContainsKey(descriptorName))
            {
                return mapping[descriptorName];
            }

            return null;
        }


    }
}
