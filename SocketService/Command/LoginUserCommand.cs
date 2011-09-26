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
using SocketService.Framework.Client.Serialize;

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
            // authenticate
            if (!UserActionEngine.Instance.LoginUser(_clientId, _username))
            {
                MSMQQueueWrapper.QueueCommand(
                    new SendObjectCommand(_clientId,
                        new LoginResponse() { Success = false })
                );

                return;
            }


            // get/create default room
            Room room = RoomActionEngine.Instance.CreateRoom(RoomActionEngine.DefaultRoom);

            User user = UserRepository.Instance.Query(u => u.ClientId.Equals(_clientId)).FirstOrDefault();
            if (user != null)
            {
                //UserActionEngine.Instance.ClientChangeRoom(_clientId, RoomActionEngine.DefaultRoom);

                // tell clients about new user
                MSMQQueueWrapper.QueueCommand(
                    new BroadcastObjectCommand(
                        room.Users.
                            Where((u) => { return u.ClientId != _clientId; }).
                            Select((u) => { return u.ClientId; }).
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
            MSMQQueueWrapper.QueueCommand(
                new SendObjectCommand(_clientId,
                    new JoinRoomEvent()
                    {
                        RoomName = room.Name,
                        RoomId = room.Id,
                        Protected = false,
                        Hidden = false,
                        Capacity = -1,
                        RoomDescription = "",
                        RoomVariables = room.RoomVariables.Select( 
                            (rv) => 
                            { 
                                return ObjectSerialize.Deserialize<SharedObject>(rv.Value); 
                            }).ToArray(),
                        Users = room.Users.Select(
                            (u) => 
                            { 
                                return new UserListEntry() 
                                { 
                                    UserName = u.Name 
                                }; 
                            }).ToArray()
                    }
                )
            );
        }
    }
        
}
