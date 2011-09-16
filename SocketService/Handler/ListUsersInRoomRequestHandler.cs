using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using SocketService.ServiceHandler;
using SocketService.Request;
using SocketService.Messaging;
using SocketService.Command;
using SocketService.Response;
using SocketService.SharedObjects;
using SocketService.Data;
using SocketService.Data.Domain;

namespace SocketService.Handler
{
    [Serializable()]
    [ServiceHandlerType(typeof(ListUsersInRoomRequest))]
    [Export(typeof(IServiceHandler))]
    public class ListUsersInRoomRequestHandler : BaseHandler<ListUsersInRoomRequest, Guid>
    {
        public override bool HandleRequest(ListUsersInRoomRequest request, Guid state)
        {
            List<User> userList = new List<User>();
            if (string.IsNullOrEmpty(request.RoomName))
            {
                User user = UserRepository.Instance.FindUserByClientKey(state);
                if (user != null)
                {
                    userList = UserRepository.Instance.FindUsersByRoom(user.Room);
                }
            }
            else
            {
                userList = UserRepository.Instance.FindUsersByRoom(request.RoomName);
            }

            var query = from u in userList
                        select new ServerUser() { Name = u.UserName };

            MSMQQueueWrapper.QueueCommand(
                new SendObjectCommand(state,
                    new ListUsersInRoomResponse() { Users = query.ToArray() })
            );

            return true;
        }
    }
}
