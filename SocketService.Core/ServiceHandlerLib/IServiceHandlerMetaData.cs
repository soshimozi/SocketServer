using System;

namespace SocketService.Core.ServiceHandlerLib
{
    public interface IServiceHandlerMetaData
    {
        Type HandlerType
        {
            get;
        }
    }
}
