using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketServer.Core.Configuration
{
    public class RequestHandlerConfigurationSection : ConfigurationSection
    {
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
    }
}
