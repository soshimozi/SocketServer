using System;
using System.Linq;
using SocketServer.Data;
using SocketServer.Repository;
using User = SocketServer.Data.User;

namespace SocketServer.Actions
{
    class UserActionEngine : SingletonBase<UserActionEngine>
    {
        public void LogoutUser(Guid clientId)
        {
            var user = UserRepository.Instance.Query(u => u.ClientKey.Equals(clientId)).FirstOrDefault();
            if (user == null) return;
            if (user.Room != null)
            {
                user.Room.Users.Remove(user);
            }

            UserRepository.Instance.Delete(user);
        }

        public bool LoginUser(Guid clientId, string loginName, Room room)
        {
            // check for duplicates
            var duplicateUser = UserRepository.Instance.Query(u => u.Name.Equals(loginName)).FirstOrDefault();
            if (duplicateUser != null)
            {
                return false;
            }

            var user = new User { ClientKey = clientId, Name = loginName };
            UserRepository.Instance.Add(user);

            room.Users.Add(user);

            RoomRepository.Instance.Update(room);
            UserRepository.Instance.Update(user);

            return true;
        }

        public bool ClientChangeRoom(Guid clientId, string roomName)
        {
            var user = UserRepository.Instance.Query(u => u.ClientKey.Equals(clientId)).FirstOrDefault();
            var room = RoomRepository.Instance.Query(r => r.Name.Equals(roomName)).FirstOrDefault();

            if (user == null || room == null) return false;

            // remove from old room
            if (user.Room != null)
            {
                user.Room.Users.Remove(user);
            }

            room.Users.Add(user);
            RoomRepository.Instance.Update(room);
            UserRepository.Instance.Update(user);

            return true;
        }

        public void RemoveAllUsers()
        {
            var users = UserRepository.Instance.GetAll();
            foreach (var user in users)
            {
                if (user.Room != null)
                {
                    user.Room.Users.Remove(user);
                }

                UserRepository.Instance.Delete(user);
            }
        }
    }
}
