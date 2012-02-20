using System;
using System.Linq;
using SocketServer.Actions;
using SocketServer.Data;
using SocketServer.Repository;
using SocketServer.Shared;
using SocketServer.Shared.Response;
using SocketServer.Shared.Serialization;
using SocketServer.Net.Client;
using SocketServer.Shared.Event;

namespace SocketServer.Command
{
    [Serializable]
    public class LoginUserCommand : BaseCommandHandler
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

            //// get/create default zone
            //var defaultZone = ZoneActionEngine.Instance.CreateZone(ZoneActionEngine.DefaultZone);

            //// get/create default room
            //var defaultRoom = RoomActionEngine.Instance.CreateRoom(RoomActionEngine.DefaultRoom, defaultZone);

            //var connection = ConnectionRepository.Instance.Query( c => c.ClientId == _clientId ).FirstOrDefault();

            //if (connection != null)
            //{
            //    // authenticate
            //    bool success = UserActionEngine.Instance.LoginUser(_clientId, _username, defaultRoom);
            //    LoginResponse response = new LoginResponse() { Success = success, UserName = _username };
            //    MSMQQueueWrapper.QueueCommand(
            //        new SendServerResponseCommand(
            //            _clientId,
            //            XmlSerializationHelper.Serialize<LoginResponse>(response),
            //            ResponseTypes.LoginResponse,
            //            connection.RequestHeader.MessageHeader));

            //    if (success)
            //    {
            //        MSMQQueueWrapper
            //            .QueueCommand
            //            (
            //                new CreateRoomCommand(
            //                    _clientId, 
            //                    ZoneActionEngine.DefaultZone, 
            //                    RoomActionEngine.DefaultRoom)
            //            );

            //        //    success = UserActionEngine.Instance.ClientChangeRoom(_clientId, defaultRoom.Name);



            //        //if (success)
            //        //{
            //        //    var user = UserRepository.Instance.Query(u => u.ClientKey == _clientId).FirstOrDefault();

            //        //    // send join room event
            //        //    JoinRoomEvent evt = new JoinRoomEvent()
            //        //    {
            //        //        RoomName = defaultRoom.Name,
            //        //        RoomId = defaultRoom.Id,
            //        //        Capacity = defaultRoom.Capacity,
            //        //        Hidden = defaultRoom.IsPrivate,
            //        //        Protected = defaultRoom.IsPersistable,
            //        //        RoomDescription = string.Empty,
            //        //        Users = defaultRoom.Users.Select(u => u.Name).ToList()
            //        //    };

            //        //    MSMQQueueWrapper.QueueCommand(
            //        //        new SendServerResponseCommand(
            //        //            _clientId,
            //        //            XmlSerializationHelper.Serialize<JoinRoomEvent>(evt),
            //        //            ResponseTypes.JoinRoomEvent,
            //        //            connection.RequestHeader.MessageHeader));

            //        //    RoomUserUpdateEvent roomUpdateEvent = new RoomUserUpdateEvent()
            //        //    {
            //        //        RoomName = defaultRoom.Name,
            //        //        RoomId = defaultRoom.Id,
            //        //        Action = RoomUserUpdateAction.AddUser,
            //        //        UserName = user.Name,
            //        //        ZoneId = defaultZone.Id
            //        //    };

            //        //    MSMQQueueWrapper.QueueCommand(
            //        //        new BroadcastMessageCommand(
            //        //            ConnectionRepository
            //        //            .Instance
            //        //            .Query(c => c.ClientId != _clientId)
            //        //            .Select(c1 => c1.ClientId).ToArray(),
            //        //            XmlSerializationHelper.Serialize<RoomUserUpdateEvent>(roomUpdateEvent),
            //        //            ResponseTypes.RoomUserUpdateEvent,
            //        //            connection.RequestHeader.MessageHeader));

            //    }
            //}


            //User user = UserRepository.Instance.Query(u => u.ClientKey.Equals(_clientId)).FirstOrDefault();
            //if (user != null)
            //{


                //UserActionEngine.Instance.ClientChangeRoom(_clientId, RoomActionEngine.DefaultRoom);

                // tell clients about new user
                //if (room != null)
                //    MSMQQueueWrapper.QueueCommand(
                //        new BroadcastMessageCommand<RoomUserUpdateEvent>(
                //            room.Users.Where(u =>
                //                                 {
                //                                     if (u == null) throw new ArgumentNullException("u");
                //                                     return u.ClientKey != _clientId;
                //                                 }).
                //                Select(u => u != null ? u.ClientKey : new Guid()).
                //                ToArray(),
                //            new RoomUserUpdateEvent
                //                {
                //                    Action = RoomUserUpdateAction.AddUser,
                //                    RoomId = room.Id,
                //                    RoomName = room.Name,
                //                    UserName = user.Name
                //                }
                //            )
                //        );
            

            // send login response
            //MSMQQueueWrapper.QueueCommand(
            //    new SendMessageCommand(_clientId,
            //                            XmlSerializationHelper.Serialize<LoginResponse>(new LoginResponse {UserName = _username, Success = true}))
            //    );

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
