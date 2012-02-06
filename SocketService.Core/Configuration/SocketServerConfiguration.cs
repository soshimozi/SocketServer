using System.Configuration;

namespace SocketServer.Core.Configuration
{
    public class SocketServerConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("Plugins", IsRequired = true, IsDefaultCollection = true)]
        public PluginInfoInstanceCollection Plugins
        {
            get { return (PluginInfoInstanceCollection) this["Plugins"]; }
            set { this["Plugins"] = value; }
        }

        [ConfigurationProperty("ListenPort", IsRequired = true)]
        public int ListenPort
        {
            get { return (int) this["ListenPort"]; }
            set { this["ListenPort"] = value; }
        }

        [ConfigurationProperty("Handlers", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(RequestHandlerCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public RequestHandlerCollection Handlers
        {
            get
            {
                return (RequestHandlerCollection)base["Handlers"];
            }
        }

        [ConfigurationProperty("Queues", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(MessageQueueCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public MessageQueueCollection Queues
        {
            get
            {
                return (MessageQueueCollection)base["Queues"];
            }
        }
    }
}