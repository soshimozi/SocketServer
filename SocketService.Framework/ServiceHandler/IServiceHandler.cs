using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace SocketService.Framework.ServiceHandler
{ 
    public interface IServiceHandler
    {
        bool HandleRequest(object request, object state);
    }
}
