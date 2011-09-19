using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.Messaging;
using SocketService.Actions;
using SocketService.Framework.Client.Data.Domain;
using SocketService.Framework.Client.Data;
using SocketService.Framework.Client.Response;
using SocketService.Framework.Client.Event;
using SocketService.Framework.SharedObjects;

namespace SocketService.Command
{
    [Serializable]
    class CreateRoomCommand : BaseMessageHandler
    {
        private readonly Guid _clientId;
        private readonly string _roomName;
        public CreateRoomCommand(Guid clientId, string roomName)
        {
            _clientId = clientId;
            _roomName = roomName;
        }

        public override void Execute()
        {
            RoomActionEngine.Instance.CreateRoom(_roomName);

            User user = UserRepository.Instance.FindUserByClientKey(_clientId);

            if (user != null && user.Room.Name != _roomName)
            {
                Room oldRoom = user.Room;

                Room room = RoomRepository.Instance.FindByName(_roomName);
                if (room == null)
                {
                    room = new Room() { Name = _roomName, Id = Room.NextId() };
                    room.AddUser(new UserListEntry() { UserName = user.UserName });

                    RoomRepository.Instance.AddRoom(room);
                }

                UserActionEngine.Instance.ClientChangeRoom(_clientId, _roomName);

                var query = from u in UserRepository.Instance.FindUsersByRoom(_roomName)
                            where u.ClientKey != _clientId
                            select u.ClientKey;

                // broadcast event
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
                            RoomVariables = room.Variables,
                            Users = room.Users
                        }
                    )
                );

                MSMQQueueWrapper.QueueCommand(
                    new BroadcastObjectCommand(query.ToArray(),
                        new RoomUserUpdateEvent()
                        {
                            Action = RoomUserUpdateAction.AddUser,
                            RoomId = room.Id,
                            UserName = user.UserName
                        }
                    )
                );

                query = from u in UserRepository.Instance.FindUsersByRoom(oldRoom.Name)
                        where u.ClientKey != _clientId
                        select u.ClientKey;

                MSMQQueueWrapper.QueueCommand(
                    new BroadcastObjectCommand(query.ToArray(),
                        new RoomUserUpdateEvent()
                        {
                            Action = RoomUserUpdateAction.DeleteUser,
                            RoomId = room.Id,
                            UserName = user.UserName
                        }
                    )
                );

                //MSMQQueueWrapper.QueueCommand(
                //    new BroadcastObjectCommand(query.ToArray(),
                //        new ServerMessage("{0} has entered the room.", user.UserName))
                //);

                //roomUsers = UserRepository.Instance.FindUsersByRoom(oldRoom);
                //query = from u in roomUsers
                //        where u.ClientKey != _clientId
                //        select u.ClientKey;

                //// tell users in old room that this user left
                //MSMQQueueWrapper.QueueCommand(
                //    new BroadcastObjectCommand(query.ToArray(),
                //        new ServerMessage("{0} has left the room.", user.UserName))
                //);

                // tell user that they entered the new room
                //MSMQQueueWrapper.QueueCommand(
                //    new SendObjectCommand(_clientId,
                //        new ServerMessage("You have entered {0}.", _roomName))
                //);
            }
        }
    }
}
