using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Net;
using SocketService.Command;
using SocketService.Framework.Util;
using SocketService.Framework.Client.Data.Domain;
using SocketService.Framework.Client.Data;
using SocketService.Framework.SharedObjects;

namespace SocketService.Actions
{
    class UserActionEngine : SingletonBase<UserActionEngine>
    {
        public void LogoutUser(Guid clientId)
        {
            User user = UserRepository.Instance.FindUserByClientKey(clientId);
            if (user != null)
            {
                UserRepository.Instance.RemoveUser(user);
            }
        }

        public bool LoginUser(Guid clientId, string loginName)
        {
            // check for duplicates
            User duplicateUser = UserRepository.Instance.FindUserByName(loginName);
            if (duplicateUser != null)
            {
                return false;
            }
            else
            {
                Room room = RoomRepository.Instance.FindByName("");
                if (room == null)
                {
                    room = new Room() { Name = "" };
                    // add default room
                    RoomRepository.Instance.AddRoom(room);
                }

                User newUser = new User() { ClientKey = clientId, UserName = loginName };
                UserRepository.Instance.AddUser(newUser);

                ClientChangeRoom(clientId, "");
                return true;
            }
        }

        public void ClientChangeRoom(Guid clientId, string roomName)
        {
            Room room = RoomRepository.Instance.FindByName(roomName);
            User user = UserRepository.Instance.FindUserByClientKey(clientId);
            if (user != null)
            {
                user.Room = room;
                room.AddUser(new UserListEntry() { UserName = user.UserName });
            }
        }
    }
}
