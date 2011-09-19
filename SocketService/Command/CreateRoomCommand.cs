﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.Messaging;
using SocketService.Actions;
using SocketService.Framework.Data;
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
            Room newRoom = RoomActionEngine.Instance.CreateRoom(_roomName);
            User user = UserRepository.Instance.FindUserByClientKey(_clientId);

            if (user != null && user.Room.Name != _roomName)
            {
                Room oldRoom = user.Room;

                if (oldRoom.Id != newRoom.Id)
                {
                    newRoom.AddUser(new UserListEntry() { UserName = user.UserName });
                    oldRoom.RemoveUser(new UserListEntry() { UserName = user.UserName });

                    UserActionEngine.Instance.ClientChangeRoom(_clientId, _roomName);

                    MSMQQueueWrapper.QueueCommand(
                        new SendObjectCommand(_clientId,
                            new JoinRoomEvent()
                            {
                                RoomName = newRoom.Name,
                                RoomId = newRoom.Id,
                                Protected = false,
                                Hidden = false,
                                Capacity = -1,
                                RoomDescription = "",
                                RoomVariables = newRoom.Variables,
                                Users = newRoom.Users
                            }
                        )
                    );

                    // tell clients to add user to room
                    MSMQQueueWrapper.QueueCommand(
                        new BroadcastObjectCommand(
                            UserRepository.Instance.FindClientKeysByRoomFiltered(_roomName, _clientId).ToArray(),
                            new RoomUserUpdateEvent()
                            {
                                Action = RoomUserUpdateAction.AddUser,
                                RoomId = newRoom.Id,
                                UserName = user.UserName
                            }
                        )
                    );

                    // tell clients to remove user from room
                    MSMQQueueWrapper.QueueCommand(
                        new BroadcastObjectCommand(
                            UserRepository.Instance.FindClientKeysByRoomFiltered(oldRoom.Name, _clientId).ToArray(),
                            new RoomUserUpdateEvent()
                            {
                                Action = RoomUserUpdateAction.DeleteUser,
                                RoomId = oldRoom.Id,
                                UserName = user.UserName
                            }
                        )
                    );
                }
            }
        }
    }
}
