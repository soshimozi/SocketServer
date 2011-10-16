using System;
using System.Collections.Generic;
using SocketService.Framework.Messaging;
using SocketService.Framework.ServiceHandlerLib;

namespace SocketService.Command
{
    [Serializable]
    public class HandleClientRequestCommand : BaseMessageHandler
    {
        private readonly List<IServiceHandler> _handlerList;
        private readonly Guid _clientConnect;
        private readonly object _request;

        public HandleClientRequestCommand(Guid connection, object request, List<IServiceHandler> handlerList)
        {
            _clientConnect = connection;
            _handlerList = handlerList;
            _request = request;
        }

        public override void Execute()
        {
            var list = _handlerList;
            foreach (var handler in list)
            {
                handler.HandleRequest(_request, _clientConnect);
            }
        }
    }
}
