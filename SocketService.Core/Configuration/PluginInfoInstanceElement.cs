using System.Configuration;

namespace SocketServer.Core.Configuration
{
    public class PluginInfoInstanceElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string) base["name"]; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("path", IsRequired = true)]
        public string Path
        {
            get { return (string) base["path"]; }
            set { base["path"] = value; }
        }
    }
}