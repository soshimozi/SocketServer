using System;
using System.Linq;
using SocketServer.Command;
using SocketServer.Messaging;
using SocketServer.Repository;
using SocketServer.Shared.Request;

namespace SocketServer.Handler
{
    public class PublicMessageRequestHandler : IRequestHandler<PublicMessageRequest>
    {
        public void HandleRequest(PublicMessageRequest request, Guid state)
        {
            var user = UserRepository.Instance.Query(u => u.ClientKey.Equals(state)).FirstOrDefault();
            if (user != null)
            {
                MSMQQueueWrapper.QueueCommand(
                    new PublicMessageCommand(request.ZoneId, request.RoomId, user.Name, request.Message)
                    );
            }
        }
    }
}