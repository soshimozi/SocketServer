using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace TestServer.Configuration
{
    public class RequestHandlerCollection : ConfigurationElementCollection
    {
        public RequestHandlerCollection()
        {
        }

        public RequestHandlerConfigurationElement this[int index]
        {
            get { return (RequestHandlerConfigurationElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public void Add(RequestHandlerConfigurationElement serviceConfig)
        {
            BaseAdd(serviceConfig);
        }

        public void Clear()
        {
            BaseClear();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new RequestHandlerConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RequestHandlerConfigurationElement)element).RequestType;
        }

        public void Remove(RequestHandlerConfigurationElement serviceConfig)
        {
            BaseRemove(serviceConfig.RequestType);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }
    }
}
