using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using SocketService.Framework.Client.Response;
using SocketService.Framework.Client.SharedObjects;
using SocketService.Framework.Data.Domain;
using SocketService.Framework.Data;
using SocketService.Framework.Messaging;
using SocketService.Framework.ServiceHandler;
using SocketService.Framework.Client.Request;
using SocketService.Framework.Command;

namespace SocketService.Handler
{
    [Serializable()]
    [ServiceHandlerType(typeof(ListUsersInRoomRequest))]
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
