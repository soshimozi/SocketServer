using System;
using System.Linq;
using SocketService.Command;
using SocketService.Framework.Client.Request;
using SocketService.Framework.Messaging;
using SocketService.Framework.ServiceHandlerLib;
using SocketService.Repository;

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