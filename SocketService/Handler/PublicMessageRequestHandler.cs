using System;
using System.Linq;
using SocketServer.Command;
using SocketServer.Core.Messaging;
using SocketServer.Core.ServiceHandlerLib;
using SocketServer.Repository;
using SocketServer.Shared.Request;

namespace SocketServer.Handler
{
    [ServiceHandlerType(typeof (PublicMessageRequest))]
    public class PublicMessageRequestHandler : BaseHandler<PublicMessageRequest, Guid>
    {
        public override bool HandleRequest(PublicMessageRequest request, Guid state)
        {
            var user = UserRepository.Instance.Query(u => u.ClientKey.Equals(state)).FirstOrDefault();
            if (user != null)
            {
                MSMQQueueWrapper.QueueCommand(
                    new PublicMessageCommand(request.ZoneId, request.RoomId, user.Name, request.Message)
                    );
            }

            return true;
        }
    }
}