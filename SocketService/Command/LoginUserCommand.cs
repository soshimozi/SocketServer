using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.Messaging;
using SocketService.Actions;
using SocketService.Framework.Data;
using SocketService.Framework.Client.Response;
using SocketService.Framework.SharedObjects;
using SocketService.Framework.Client.Event;
using SocketService.Repository;

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
                // get default room
                Room room = RoomActionEngine.Instance.CreateRoom("");

                User user = UserRepository.Instance.Query(u => u.ClientId.Equals(_clientId)).FirstOrDefault();
                if (user != null)
                {
                    UserActionEngine.Instance.ClientChangeRoom(_clientId, "");

                    // tell clients to add user to room
                    MSMQQueueWrapper.QueueCommand(
                        new BroadcastObjectCommand(
                            room.Users.
                                Where( (u) => { return u.ClientId != _clientId; } ).
                                Select( (u1) => { return u1.ClientId; } ).
                                ToArray(),
                            new RoomUserUpdateEvent()
                            {
                                Action = RoomUserUpdateAction.AddUser,
                                RoomId = room.Id,
                                UserName = user.Name
                            }
                        )
                    );
                }

                // send login response
                MSMQQueueWrapper.QueueCommand(
                    new SendObjectCommand(_clientId,
                        new LoginResponse() { UserName = _username, Success = true })
                );

                // finally send a join room event to user
                //MSMQQueueWrapper.QueueCommand(
                //    new SendObjectCommand(_clientId,
                //        new JoinRoomEvent()
                //        {
                //            RoomName = "",
                //            RoomId = room.Id,
                //            Protected = false,
                //            Hidden = false,
                //            Capacity = -1,
                //            RoomDescription = "",
                //            RoomVariables = room.RoomVariables.ToArray(),
                //            Users = room.Users.ToArray()
                //        }
                //    )
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
