using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace SocketServer.Core.Configuration
{
    public class RequestHandlerConfigurationElement : ConfigurationElement
    {
        public RequestHandlerConfigurationElement()
        {
        }

        public RequestHandlerConfigurationElement(String requestTypeTag, String requestType, String handlerType)
        {
            RequestTypeTag = requestTypeTag;
            HandlerType = handlerType;
            RequestType = requestType;
        }

        [ConfigurationProperty("RequestTypeTag", IsRequired = true)]
        public String RequestTypeTag
        {
            get
            { return (String)this["RequestTypeTag"]; }
            set
            { this["RequestTypeTag"] = value; }
        }

        [ConfigurationProperty("RequestType", IsRequired = true)]
        public String RequestType
        {
            get
            { return (String)this["RequestType"]; }
            set
            { this["RequestType"] = value; }
        }

        [ConfigurationProperty("HandlerType", IsRequired = true)]
        public String HandlerType
        {
            get
            { return (String)this["HandlerType"]; }
            set
            { this["HandlerType"] = value; }
        }
    }
}
