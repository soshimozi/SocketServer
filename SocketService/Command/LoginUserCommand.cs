using System;
using System.Linq;
using SocketServer.Actions;
using SocketServer.Core.Data;
using SocketServer.Core.Messaging;
using SocketServer.Event;
using SocketServer.Repository;
using SocketServer.Shared;
using SocketServer.Shared.Response;

namespace SocketServer.Command
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
            Logger.InfoFormat("Client {0} logging in.", _clientId);

            // get/create default zone
            var zone = ZoneActionEngine.Instance.CreateZone(ZoneActionEngine.DefaultZone);

            // get/create default room
            var room = RoomActionEngine.Instance.CreateRoom(RoomActionEngine.DefaultRoom, zone);

            // authenticate
            if (!UserActionEngine.Instance.LoginUser(_clientId, _username, room))
            {
                MSMQQueueWrapper.QueueCommand(
                    new SendMessageCommand<LoginResponse>(_clientId,
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
                        new BroadcastMessageCommand<RoomUserUpdateEvent>(
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
                new SendMessageCommand<LoginResponse>(_clientId,
                                      new LoginResponse {UserName = _username, Success = true})
                );

            // finally send a join room event to user
            //if (room != null)
            //    MSMQQueueWrapper.QueueCommand(
            //        new SendObjectCommand(_clientId,
            //                              new JoinRoomEvent
            //                                  {
            //                                      RoomName = room.Name,
            //                                      RoomId = room.Id,
            //                                      Protected = false,
            //                                      Hidden = false,
            //                                      Capacity = -1,
            //                                      RoomDescription = "",
            //                                      RoomVariables = room.RoomVariables.Select(
            //                                          rv =>
            //                                          ObjectSerialize.Deserialize(rv.Value)).
            //                                          ToArray(),
            //                                      Users = room.Users.Select(
            //                                          u => u.Name
            //                                          ).ToArray()
            //                                  }
            //            )
            //        );
        }
    }
}
