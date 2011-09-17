using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using SocketService.Framework.Response;
using SocketService.Framework.Messaging;
using SocketService.Framework.ServiceHandler;
using SocketService.Framework.Request;
using SocketService.Framework.Net.Client;
using SocketService.Framework.Command;

namespace SocketService.Handler
{
    [Serializable()]
    [ServiceHandlerType(typeof(NegotiateKeysRequest))]
    public class NegotiateKeysRequestHandler : 
        BaseHandler<NegotiateKeysRequest, Guid>
    {
        public override bool HandleRequest(NegotiateKeysRequest request, Guid state)
        {
            NegotiateKeysRequest negotiateKeysRequest = request as NegotiateKeysRequest;
            Guid clientId = (Guid)state;

            Connection connection = ConnectionRepository.Instance.FindConnectionByClientId(clientId);
            if (connection != null)
            {
                // import clients public key
                connection.RemotePublicKey = connection.Provider.Import(negotiateKeysRequest.PublicKey);

                // send our public key back
                NegotiateKeysResponse response = new NegotiateKeysResponse();
                response.RemotePublicKey = connection.Provider.PublicKey.ToByteArray();

                // now we send a response back
                MSMQQueueWrapper.QueueCommand(new SendObjectCommand(clientId, response));
            }

            return true;
        }
    }
}
