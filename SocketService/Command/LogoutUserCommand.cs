using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Client;
using SocketService.Net.Sockets;
using SocketService.Net;
using SocketService.Response;
using SocketService.Framework.Actions;
using SocketService.Framework.Messaging;
using SocketService.Framework.Data.Domain;
using SocketService.Framework.Data;

namespace SocketService.Command
{
    [Serializable]
    class LogoutUserCommand : BaseMessageHandler
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
