using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace SocketServer.Configuration
{
    public class MessageQueueCollection : ConfigurationElementCollection
    {
        public MessageQueueCollection()
        {
        }

        public MessageQueueConfigurationElement this[int index]
        {
            get { return (MessageQueueConfigurationElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public void Add(MessageQueueConfigurationElement element)
        {
            BaseAdd(element);
        }

        public void Clear()
        {
            BaseClear();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new MessageQueueConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MessageQueueConfigurationElement)element).QueueName;
        }

        public void Remove(MessageQueueConfigurationElement element)
        {
            BaseRemove(element.QueueName);
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
