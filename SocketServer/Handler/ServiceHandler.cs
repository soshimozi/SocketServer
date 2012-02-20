using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace SocketServer.Handler
{
    public class ServiceHandler
    {
        private readonly object _handler;
        private readonly MethodInfo _method;

        public ServiceHandler(object handler, MethodInfo method)
        {
            _handler = handler;
            _method = method;
        }

        public void Invoke(params object [] parameters)
        {
            _method.Invoke(_handler, parameters);
        }
    }
}
