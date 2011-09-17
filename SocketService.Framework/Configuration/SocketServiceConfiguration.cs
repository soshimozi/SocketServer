using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace SocketService.Framework.Configuration
{
    public class SocketServiceConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("Plugins", IsRequired= true, IsDefaultCollection= true)]
        public PluginInfoInstanceCollection Plugins
        {
            get { return (PluginInfoInstanceCollection)this["Plugins"]; }
            set { this["Plugins"] = value; }
        }

        [ConfigurationProperty("ListenPort", IsRequired = true)]
        public int ListenPort
        {
            get { return (int)this["ListenPort"]; }
            set { this["ListenPort"] = value; }
        }
    
    }
}
