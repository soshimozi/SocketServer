using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Framework.ServiceHandler
{
    public interface IServiceHandlerMetaData
    {
        Type HandlerType
        {
            get;
        }
    }
}
