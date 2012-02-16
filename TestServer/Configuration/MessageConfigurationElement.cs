using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace TestServer.Configuration
{
    public class MessageConfigurationElement: ConfigurationElement
    {
        public MessageConfigurationElement()
        {
        }

        public MessageConfigurationElement(String key, String typeName)
        {
            Name = key;
            TypeName = typeName;
        }

        [ConfigurationProperty("name", IsRequired = true)]
        public String Name
        {
            get
            { return (String)this["name"]; }
            set
            { this["name"] = value; }
        }

        [ConfigurationProperty("typename", IsRequired = true)]
        public String TypeName
        {
            get
            { return (String)this["typename"]; }
            set
            { this["typename"] = value; }
        }

    }
}
