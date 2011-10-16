using System;

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
