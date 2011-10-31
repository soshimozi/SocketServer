using System;
using System.Linq;
using SocketService.Actions;
using SocketService.Framework.Client.Event;
using SocketService.Framework.Client.Response;
using SocketService.Framework.Client.Serialize;
using SocketService.Framework.Client.SharedObjects;
using SocketService.Framework.Data;
using SocketService.Framework.Messaging;
using SocketService.Repository;

namespace SocketService.Command
{
    [Serializable]
    public class LoginUserCommand : BaseMessageHandler
    {
        private readonly Guid _clientId;
        private readonly string _username;

        public LoginUserCommand(Guid clientId, string username)
        {
            _clientId = clientId;
            _username = username;
        }

        public override void Execute()
        {
            // get/create default zone
            Zone zone = ZoneActionEngine.Instance.CreateZone(ZoneActionEngine.DefaultZone);

            // get/create default room
            Room room = RoomActionEngine.Instance.CreateRoom(RoomActionEngine.DefaultRoom, zone);

            // authenticate
            if (!UserActionEngine.Instance.LoginUser(_clientId, _username, room))
            {
                MSMQQueueWrapper.QueueCommand(
                    new SendObjectCommand(_clientId,
                                          new LoginResponse {Success = false})
                    );

                return;
            }


            User user = UserRepository.Instance.Query(u => u.ClientKey.Equals(_clientId)).FirstOrDefault();
            if (user != null)
            {
                UserActionEngine.Instance.ClientChangeRoom(_clientId, RoomActionEngine.DefaultRoom);

                // tell clients about new user
                if (room != null)
                    MSMQQueueWrapper.QueueCommand(
                        new BroadcastObjectCommand(
                            room.Users.Where(u =>
                                                 {
                                                     if (u == null) throw new ArgumentNullException("u");
                                                     return u.ClientKey != _clientId;
                                                 }).
                                Select(u => u != null ? u.ClientKey : new Guid()).
                                ToArray(),
                            new RoomUserUpdateEvent
                                {
                                    Action = RoomUserUpdateAction.AddUser,
                                    RoomId = room.Id,
                                    RoomName = room.Name,
                                    UserName = user.Name
                                }
                            )
                        );
            }

            // send login response
            MSMQQueueWrapper.QueueCommand(
                new SendObjectCommand(_clientId,
                                      new LoginResponse {UserName = _username, Success = true})
                );

            // finally send a join room event to user
            if (room != null)
                MSMQQueueWrapper.QueueCommand(
                    new SendObjectCommand(_clientId,
                                          new JoinRoomEvent
                                              {
                                                  RoomName = room.Name,
                                                  RoomId = room.Id,
                                                  Protected = false,
                                                  Hidden = false,
                                                  Capacity = -1,
                                                  RoomDescription = "",
                                                  RoomVariables = room.RoomVariables.Select(
                                                      rv =>
                                                      ObjectSerialize.Deserialize<SharedObject>(rv.Value)).
                                                      ToArray(),
                                                  Users = room.Users.Select(
                                                      u =>
                                                      new UserListEntry {UserName = u.Name}).
                                                      ToArray()
                                              }
                        )
                    );
        }
    }
}