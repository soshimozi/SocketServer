using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using SocketService.Framework.Messaging;
using SocketService.Framework.ServiceHandler;
using SocketService.Framework.Client.Request;
using SocketService.Framework.Client.Crypto;
using SocketService.Framework.Net.Client;
using SocketService.Framework.Command;

namespace SocketService.Handler
{
    [Serializable()]
    [ServiceHandlerType(typeof(GetCentralAuthorityRequest))]
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
