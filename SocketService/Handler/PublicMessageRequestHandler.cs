using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.ServiceHandlerLib;
using SocketService.Framework.Request;
using SocketService.Framework.Messaging;
using SocketService.Command;
using SocketService.Framework.Data;
using SocketService.Repository;

namespace SocketService.Handler
{
    [ServiceHandlerType(typeof(PublicMessageRequest))]
    public class PublicMessageRequestHandler : BaseHandler<PublicMessageRequest, Guid>
    {
        public override bool HandleRequest(PublicMessageRequest request, Guid state)
        {
            User user = UserRepository.Instance.Query( u => u.ClientKey.Equals(state) ).FirstOrDefault();
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
