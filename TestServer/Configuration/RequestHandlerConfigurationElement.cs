using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace TestServer.Configuration
{
    public class RequestHandlerConfigurationElement : ConfigurationElement
    {
        public RequestHandlerConfigurationElement()
        {
        }

        public RequestHandlerConfigurationElement(String requestTypeTag, String requestType, String handlerType)
        {
            Key = requestTypeTag;
            HandlerType = handlerType;
            RequestType = requestType;
        }

        [ConfigurationProperty("key", IsRequired = true)]
        public String Key
        {
            get
            { return (String)this["key"]; }
            set
            { this["key"] = value; }
        }

        [ConfigurationProperty("requestType", IsRequired = true)]
        public String RequestType
        {
            get
            { return (String)this["requestType"]; }
            set
            { this["requestType"] = value; }
        }

        [ConfigurationProperty("handlerType", IsRequired = true)]
        public String HandlerType
        {
            get
            { return (String)this["handlerType"]; }
            set
            { this["handlerType"] = value; }
        }
    }
}
