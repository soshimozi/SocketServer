using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.Data;

namespace SocketService
{
    public class UserRepository
    {
        private static UserRepository _instance = new UserRepository();

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static UserRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UserRepository();
                }

                return _instance;
            }
        }

        /// <summary>
        /// Finds the users by room.
        /// </summary>
        /// <param name="roomname">The roomname.</param>
        /// <returns></returns>
        public List<User> FindUsersByRoom(string roomname)
        {
            throw new NotImplementedException();

        }

        public List<Guid> FindClientKeysByRoomFiltered(string roomname, Guid filteredClient)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds the name of the user by.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        public User FindUserByName(string username)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds the user by client key.
        /// </summary>
        /// <param name="clientKey">The client key.</param>
        /// <returns></returns>
        public User FindUserByClientKey(Guid clientKey)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds the user.
        /// </summary>
        /// <param name="user">The user.</param>
        public void AddUser(User user)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the user.
        /// </summary>
        /// <param name="user">The user.</param>
        public void RemoveUser(User user)
        {
            throw new NotImplementedException();
        }

        public List<Guid> FindClientKeysByRoom(string RoomName)
        {
            throw new NotImplementedException();
        }
    }
}
