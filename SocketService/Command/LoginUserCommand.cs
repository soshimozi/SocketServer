using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.Messaging;
using SocketService.Actions;
using SocketService.Framework.Client.Data;
using SocketService.Framework.Client.Data.Domain;
using SocketService.Framework.Client.Response;

namespace SocketService.Command
{
    [Serializable]
    public class LoginUserCommand : BaseMessageHandler
    {
        private readonly string _username;
        private readonly Guid _clientId;
        public LoginUserCommand(Guid clientId, string username)
        {
            _clientId = clientId;
            _username = username;
        }

        public override void Execute()
        {
            if (UserActionEngine.Instance.LoginUser(_clientId, _username))
            {
                //List<User> roomUsers = UserRepository.Instance.FindUsersByRoom("");

                //// filter out our key
                //var query = from user in roomUsers
                //            where user.ClientKey != _clientId
                //            select user.ClientKey;

                MSMQQueueWrapper.QueueCommand(
                    new SendObjectCommand(_clientId,
                        new LoginResponse() { UserName = _username, Success = true })
                );

                // TODO: Replace with RoomUserUpdateEvent

                //MSMQQueueWrapper.QueueCommand(
                //    new BroadcastObjectCommand(query.ToArray(), new ServerMessage("{0} has logged in.", _username))
                //);

            }
            else
            {
                MSMQQueueWrapper.QueueCommand(
                    new SendObjectCommand(_clientId,
                        new LoginResponse() { Success = false })
                );

            }
        }
    }
}
