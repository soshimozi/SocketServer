using System.Configuration;

namespace TestServer.Configuration
{
    public class SocketServerConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("listenPort", IsRequired = true)]
        public int ListenPort
        {
            get { return (int)this["listenPort"]; }
            set { this["listenPort"] = value; }
        }

        [ConfigurationProperty("handlers", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(RequestHandlerCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public RequestHandlerCollection Handlers
        {
            get
            {
                return (RequestHandlerCollection)base["handlers"];
            }
        }

        [ConfigurationProperty("queues", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(MessageQueueCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public MessageQueueCollection Queues
        {
            get
            {
                return (MessageQueueCollection)base["queues"];
            }
        }

        [ConfigurationProperty("messages", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(MessageCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public MessageCollection Messages
        {
            get
            {
                return (MessageCollection)base["messages"];
            }
        }
    }
}