using System;
using System.Linq;
using SocketServer.Actions;
using SocketServer.Repository;
using SocketServer.Shared.Serialization;
using SocketServer.Shared;
using SocketServer.Net.Client;
using SocketServer.Shared.Event;

namespace SocketServer.Command
{
    [Serializable]
    class CreateRoomCommand : BaseCommandHandler
    {
        private readonly Guid _clientId;
        private readonly string _roomName;
        private readonly string _zoneName;
        public CreateRoomCommand(Guid clientId, string zoneName, string roomName)
        {
            _clientId = clientId;
            _roomName = roomName;
            _zoneName = zoneName;
        }

        public override void Execute()
        {
            var newZone = ZoneActionEngine.Instance.CreateZone(_zoneName);
            var connection = ConnectionRepository.Instance.Query(c => c.ClientId == _clientId).FirstOrDefault();

            if( newZone != null && connection != null)
            {
                var newRoom = RoomActionEngine.Instance.CreateRoom(_roomName, newZone);
                if (newRoom != null && UserActionEngine.Instance.ClientChangeRoom(_clientId, _roomName))
                { 
                    JoinRoomEvent evt = new JoinRoomEvent() 
                    { 
                        RoomName = newRoom.Name, 
                        RoomId = newRoom.Id, 
                        Capacity = newRoom.Capacity, 
                        Hidden = newRoom.IsPrivate, 
                        Protected = newRoom.IsPersistable, 
                        RoomDescription = string.Empty,
                        Users = newRoom.Users.Select( u => u.Name ).ToList()};

                    MSMQQueueWrapper.QueueCommand(
                        new SendServerResponseCommand(
                            _clientId,
                            XmlSerializationHelper.Serialize<JoinRoomEvent>(evt),
                            ResponseTypes.JoinRoomEvent,
                            connection.RequestHeader.MessageHeader));
                }
            }

            //Zone newZone = ZoneActionEngine.Instance.CreateZone(_zoneName);

            //if (user != null && user.Room.Name != _roomName)
            //{
            //    Room oldRoom = user.Room;

            //    if (oldRoom.Id != newRoom.Id)
            //    {
            //        newRoom.AddUser(new UserListEntry() { UserName = user.UserName });
            //        oldRoom.RemoveUser(new UserListEntry() { UserName = user.UserName });

            //        UserActionEngine.Instance.ClientChangeRoom(_clientId, _roomName);

            //        // tell client that they are leaving a room
            //        MSMQQueueWrapper.QueueCommand(
            //            new SendObjectCommand(_clientId,
            //                new LeaveRoomEvent()
            //                {
            //                    RoomId = oldRoom.Id,
            //                    UserName = user.UserName
            //                }
            //            )
            //        );

            //        // now tell client they are arriving at a new room
            //        MSMQQueueWrapper.QueueCommand(
            //            new SendObjectCommand(_clientId,
            //                new JoinRoomEvent()
            //                {
            //                    RoomName = newRoom.Name,
            //                    RoomId = newRoom.Id,
            //                    Protected = false,
            //                    Hidden = false,
            //                    Capacity = -1,
            //                    RoomDescription = "",
            //                    RoomVariables = newRoom.Variables.ToArray(),
            //                    Users = newRoom.Users.ToArray()
            //                }
            //            )
            //        );

            //        // tell clients to add user to room
            //        MSMQQueueWrapper.QueueCommand(
            //            new BroadcastObjectCommand(
            //                UserRepository.Instance.FindClientKeysByRoomFiltered(_roomName, _clientId).ToArray(),
            //                new RoomUserUpdateEvent()
            //                {
            //                    Action = RoomUserUpdateAction.AddUser,
            //                    RoomId = newRoom.Id,
            //                    UserName = user.UserName
            //                }
            //            )
            //        );

            //        // tell clients to remove user from room
            //        MSMQQueueWrapper.QueueCommand(
            //            new BroadcastObjectCommand(
            //                UserRepository.Instance.FindClientKeysByRoomFiltered(oldRoom.Name, _clientId).ToArray(),
            //                new RoomUserUpdateEvent()
            //                {
            //                    Action = RoomUserUpdateAction.DeleteUser,
            //                    RoomId = oldRoom.Id,
            //                    UserName = user.UserName
            //                }
            //            )
            //        );
            //    }
            //}
        }
    }
}
