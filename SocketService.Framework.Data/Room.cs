using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.SharedObjects;

namespace SocketService.Framework.Data
{
    public class Room
    {
        public Room()
        {
            Id = NextId();
        }

        private static int nextId = 0;
        protected static int NextId()
        {
            return nextId++;
        }

        private Dictionary<string, RoomVariable> _roomVariables = new Dictionary<string, RoomVariable>();
        private List<UserListEntry> _users = new List<UserListEntry>();

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the variable.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public RoomVariable GetVariable(string name)
        {
            if (_roomVariables.ContainsKey(name))
            {
                return _roomVariables[name];
            }

            return null;
        }

        /// <summary>
        /// Adds the variable.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="so">The so.</param>
        public void AddVariable(string name, RoomVariable so)
        {
            if (!_roomVariables.ContainsKey(name))
            {
                _roomVariables.Add(name, so);
            }
        }

        public int Id { get; private set; }

        public List<RoomVariable> Variables
        {
            get { return _roomVariables.Values.ToList(); }
        }

        public List<UserListEntry> Users { get { return _users.ToList(); } }

        public UserListEntry FindByName(string name)
        {
            var query = from user in _users
                        where user.UserName.Equals(name)
                        select user;

            return query.FirstOrDefault();
        }
    
        public void AddUser(UserListEntry user)
        {
            UserListEntry entry = FindByName(user.UserName);
            if (entry == null)
            {
                _users.Add(user);
            }
        }

        public void RemoveUser(UserListEntry user)
        {
            UserListEntry entry = FindByName(user.UserName);
            if( entry != null )
            {
                _users.Remove(entry);
            }
        }
    }
}
