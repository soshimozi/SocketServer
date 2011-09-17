using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.Util;
using SocketService.Framework.Data.Domain;

namespace SocketService.Framework.Data
{
    public class UserRepository : SingletonBase<UserRepository>
    {
        private object _listLock = new object();
        private List<User> _userList = new List<User>();

        public List<User> FindUsersByRoom(string roomname)
        {
            lock (_listLock)
            {
                var query = from user in _userList
                            where user.Room == roomname
                            select user;

                return query.ToList();
            }
        }

        public User FindUserByName(string username)
        {
            lock (_listLock)
            {
                var query = from user in _userList
                            where user.UserName == username
                            select user;

                return query.FirstOrDefault();
            }
        }

        public User FindUserByClientKey(Guid clientKey)
        {
            lock (_listLock)
            {
                var query = from user in _userList
                            where user.ClientKey == clientKey
                            select user;

                return query.FirstOrDefault();
            }
        }

        public void AddUser(User user)
        {
            lock (_listLock)
            {
                _userList.Add(user);
            }
        }

        public void RemoveUser(User user)
        {
            lock (_listLock)
            {
                _userList.Remove(user);
            }
        }
    }
}
