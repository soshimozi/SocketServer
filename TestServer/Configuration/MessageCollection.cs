using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace TestServer.Configuration
{
    public class MessageCollection: ConfigurationElementCollection
    {
        public MessageCollection()
        {
        }

        public MessageConfigurationElement this[int index]
        {
            get { return (MessageConfigurationElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public void Add(MessageConfigurationElement element)
        {
            BaseAdd(element);
        }

        public void Clear()
        {
            BaseClear();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new MessageConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MessageConfigurationElement)element).Name;
        }

        public void Remove(MessageConfigurationElement element)
        {
            BaseRemove(element.Name);
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
