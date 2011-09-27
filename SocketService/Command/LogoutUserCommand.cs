using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.Messaging;
using SocketService.Net.Client;
using SocketService.Framework.Data;
using SocketService.Framework.Client.Response;
using SocketService.Actions;
using SocketService.Repository;

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

            User user = UserRepository.Instance.Query(u => u.ClientKey.Equals(_clientId)).FirstOrDefault();

            UserActionEngine.Instance.LogoutUser(_clientId);

            if( user.Room != null )
            {
                List<User> userList = user.Room.Users.ToList();
            }

            //// broadcast to all but this user
            //var query = from u in userList
            //            where u.ClientId != _clientId
            //            select u.ClientId;

            // TODO: Replace with PublicMessageEvent
            //MSMQQueueWrapper.QueueCommand(
            //    new BroadcastObjectCommand(query.ToArray(),
            //        new ServerMessage("{0} has logged out.", user.UserName))
            //);

        }
    }
}
