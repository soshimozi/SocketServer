using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.ServiceHandler;
using System.ComponentModel.Composition;
using SocketService.Request;
using SocketService.Crypto;
using SocketService.Serial;
using SocketService.Command;
using SocketService.Client;
using SocketService.Messaging;

namespace SocketService.Handler
{
    [Export(typeof(IServiceHandler))]
    [ServiceHandlerType(typeof(GetCentralAuthorityRequest))]
    [Serializable()]
    public class GetCARequestHandler : 
        BaseHandler<GetCentralAuthorityRequest, Guid>
    {
        public override bool HandleRequest(GetCentralAuthorityRequest request, Guid state)
        {
            CentralAuthority ca = new CentralAuthority(CAKeyProtocol.DH64);
            Guid clientId = (Guid)state;

            Connection connection = ConnectionRepository.Instance.FindConnectionByClientId(clientId);
            connection.Provider = ca.GetProvider();

            MSMQQueueWrapper.QueueCommand(new SendObjectCommand(clientId, ca));
            return true;
        }
    }
}
