using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.ServiceHandler
{
    public interface IServiceHandlerType
    {
        Type HandlerType
        {
            get;
        }
    }
}
