using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Net;
using SocketService.Command;
using SocketService.Framework.Util;
using SocketService.Framework.Data;
using SocketService.Framework.SharedObjects;
using SocketService.Repository;

namespace SocketService.Actions
{
    class UserActionEngine : SingletonBase<UserActionEngine>
    {
        public void LogoutUser(Guid clientId)
        {
            User user = UserRepository.Instance.Query(u => u.ClientKey.Equals(clientId)).FirstOrDefault();
            if (user != null)
            {
                UserRepository.Instance.Delete(user);
            }
        }

        public bool LoginUser(Guid clientId, string loginName, Room entryRoom)
        {
            // check for duplicates
            User duplicateUser = UserRepository.Instance.Query(u => u.Name.Equals(loginName)).FirstOrDefault();
            if (duplicateUser != null)
            {
                return false;
            }
            else
            {
                User user = new User() { ClientKey = clientId, Name = loginName, Room = entryRoom };
                UserRepository.Instance.Add(user);

                entryRoom.Users.Add(user);

                RoomRepository.Instance.Update(entryRoom);
                UserRepository.Instance.Update(user);

                return true;
            }
        }

        public void ClientChangeRoom(Guid clientId, string roomName)
        {
            User user = UserRepository.Instance.Query(u => u.ClientKey.Equals(clientId)).FirstOrDefault();
            Room room = RoomRepository.Instance.Query(r => r.Name.Equals(roomName)).FirstOrDefault();

            if (user != null && room != null)
            {
                // remove from old room
                if (user.Room != null)
                {
                    room.Users.Remove(user);
                }

                room.Users.Add(user);
                RoomRepository.Instance.Update(room);
                UserRepository.Instance.Update(user);
            }
        }
    }
}
