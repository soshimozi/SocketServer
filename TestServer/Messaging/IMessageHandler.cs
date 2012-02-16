using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestServer.Messaging
{
    public interface IMessageHandler<TIn, TOut> where TIn : class where TOut : class
    {
        TOut Proccess(TIn Message);
    }
}
