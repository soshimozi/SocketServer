using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Framework.ServiceHandlerLib
{
    public interface IServiceHandlerMetaData
    {
        Type HandlerType
        {
            get;
        }
    }
}
