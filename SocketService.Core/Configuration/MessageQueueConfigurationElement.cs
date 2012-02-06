using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace SocketServer.Core.Configuration
{
    public class MessageQueueConfigurationElement : ConfigurationElement
    {
        public MessageQueueConfigurationElement()
        {
        }

        public MessageQueueConfigurationElement(String queueKey, String queueName)
        {
            QueueName = queueKey;
            QueuePath = queueName;
        }

        [ConfigurationProperty("name", IsRequired = true)]
        public String QueueName
        {
            get
            { return (String)this["name"]; }
            set
            { this["name"] = value; }
        }

        [ConfigurationProperty("path", IsRequired = true)]
        public String QueuePath
        {
            get
            { return (String)this["path"]; }
            set
            { this["path"] = value; }
        }

    }
}
