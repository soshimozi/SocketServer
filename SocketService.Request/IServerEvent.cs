using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Request
{
    public interface IServerEvent
    {
        string EventName
        {
            get;
            set;
        }
    }
}
