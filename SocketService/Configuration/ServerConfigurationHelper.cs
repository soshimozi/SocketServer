using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace SocketServer.Configuration
{
    public static class ServerConfigurationHelper
    {
        public static SocketServerConfiguration GetServerConfiguration()
        {
            return (SocketServerConfiguration)ConfigurationManager.
                GetSection("SocketServerConfiguration");
        }
    }
}
