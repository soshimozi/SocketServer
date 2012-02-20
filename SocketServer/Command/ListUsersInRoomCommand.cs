using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.Messaging;
using SocketService.Framework.Client.Data.Domain;
using SocketService.Framework.Client.Data;
using SocketService.Framework.SharedObjects;
using SocketService.Framework.Client.Response;

namespace SocketService.Command
{
    [Serializable]
    public class ListUsersInRoomCommand : BaseMessageHandler
    {
        private readonly string _room;
        private readonly Guid _clientId;
        public ListUsersInRoomCommand(string room, Guid clientId)
        {
            _room = room;
            _clientId = clientId;
        }


        public override void Execute()
        {
            User user = UserRepository.Instance.FindUserByClientKey(_clientId);
            if (user != null)
            {
                List<User> userList = new List<User>();
                if (string.IsNullOrEmpty(_room))
                {
                    userList = UserRepository.Instance.FindUsersByRoom(user.Room);
                }
                else
                {
                    userList = UserRepository.Instance.FindUsersByRoom(_room);
                }

                var query = from u in userList
                            select new ServerUser() { Name = u.UserName };

                MSMQQueueWrapper.QueueCommand(
                    new SendObjectCommand(_clientId,
                        new ListUsersInRoomResponse() { Users = query.ToArray() })
                );
            }
        }
    }
}
