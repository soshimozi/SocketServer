using System;
using System.Linq;
using SocketService.Command;
using SocketService.Core.Messaging;
using SocketService.Core.ServiceHandlerLib;
using SocketService.Repository;
using SocketService.Shared.Request;

namespace SocketService.Handler
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