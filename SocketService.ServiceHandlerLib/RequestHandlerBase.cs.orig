using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace ServiceHandlerLib
{
    [InheritedExport(typeof(IServiceHandler))]
    public abstract class RequestHandlerBase : IServiceHandler
    {
        public abstract bool HandleRequest(IRequest request, IServerContext server);
    }
}
