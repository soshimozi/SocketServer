using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using SocketService.Framework.Messaging;
using SocketService.Framework.ServiceHandlerLib;
using SocketService.Framework.Client.Request;
using SocketService.Framework.Client.Crypto;

namespace SocketService.Handlers
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
