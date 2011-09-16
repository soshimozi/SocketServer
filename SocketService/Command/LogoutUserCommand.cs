using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Messaging;
using SocketService.Client;
using SocketService.Net.Sockets;
using SocketService.Net;
using SocketService.Data.Domain;
using SocketService.Data;
using SocketService.Actions;
using SocketService.Response;

namespace SocketService.Command
{
    [Serializable]
    class LogoutUserCommand : BaseCommand
    {
        private readonly Guid _clientId;
        public LogoutUserCommand(Guid clientId)
        {
            _clientId = clientId;
        }

        public override void Execute()
        {
            Connection connection = ConnectionRepository.Instance.FindConnectionByClientId(_clientId);
            if (connection != null)
                ConnectionRepository.Instance.RemoveConnection(connection);

            User user = UserRepository.Instance.FindUserByClientKey(_clientId);

            UserActionEngine.Instance.LogoutUser(_clientId);

            List<User> userList = UserRepository.Instance.FindUsersByRoom(user.Room);

            // broadcast to all but this user
            var query = from u in userList
                        where u.ClientKey != _clientId
                        select u.ClientKey;

            MSMQQueueWrapper.QueueCommand(
                new BroadcastObjectCommand(query.ToArray(),
                    new ServerMessage("{0} has logged out.", user.UserName))
            );

        }
    }
}
