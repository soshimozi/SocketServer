using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.ServiceHandler
{
    public class HandlerAttribute : Attribute
    {
        Type handlerType;

        public HandlerAttribute(Type handlerType)
        {
        }
    }
}
