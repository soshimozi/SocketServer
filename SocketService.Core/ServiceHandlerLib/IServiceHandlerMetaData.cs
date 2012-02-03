using System;

namespace SocketServer.Core.ServiceHandlerLib
{
    public interface IServiceHandlerMetaData
    {
        Type HandlerType
        {
            get;
        }
    }
}
