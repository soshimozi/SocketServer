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

        /// <summary>
        /// Finds the users by room.
        /// </summary>
        /// <param name="roomname">The roomname.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Finds the name of the user by.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Finds the user by client key.
        /// </summary>
        /// <param name="clientKey">The client key.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Adds the user.
        /// </summary>
        /// <param name="user">The user.</param>
        public void AddUser(User user)
        {
            lock (_listLock)
            {
                _userList.Add(user);
            }
        }

        /// <summary>
        /// Removes the user.
        /// </summary>
        /// <param name="user">The user.</param>
        public void RemoveUser(User user)
        {
            lock (_listLock)
            {
                _userList.Remove(user);
            }
        }
    }
}
