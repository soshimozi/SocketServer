using System.Configuration;

namespace SocketServer.Configuration
{
    public class SocketServerConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("plugins", IsRequired = true, IsDefaultCollection = true)]
        public PluginInfoInstanceCollection Plugins
        {
            get { return (PluginInfoInstanceCollection)this["plugins"]; }
            set { this["plugins"] = value; }
        }

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
    }
}