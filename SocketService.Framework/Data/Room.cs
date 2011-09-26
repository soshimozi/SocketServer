using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.SharedObjects;
using SocketService.Framework.Client.API;

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

        private object _variableLock = new object();
        private object _pluginLock = new object();
        private object _userLock = new object();

        private Dictionary<string, RoomVariable> _roomVariables = new Dictionary<string, RoomVariable>();
        private Dictionary<string, IPlugin> _roomPlugins = new Dictionary<string, IPlugin>();
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
            lock (_variableLock)
            {
                if (_roomVariables.ContainsKey(name))
                {
                    return _roomVariables[name];
                }

                return null;
            }
        }

        /// <summary>
        /// Adds the variable.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="so">The so.</param>
        public void AddVariable(string name, RoomVariable so)
        {
            lock (_variableLock)
            {
                if (!_roomVariables.ContainsKey(name))
                {
                    _roomVariables.Add(name, so);
                }
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
            lock (_userLock)
            {
                var query = from user in _users
                            where user.UserName.Equals(name)
                            select user;

                return query.FirstOrDefault();
            }
        }
    
        public void AddUser(UserListEntry user)
        {
            UserListEntry entry = FindByName(user.UserName);
            if (entry == null)
            {
                lock (_userLock)
                {
                    _users.Add(user);
                }
            }
        }

        public void RemoveUser(UserListEntry user)
        {
            UserListEntry entry = FindByName(user.UserName);
            if( entry != null )
            {
                lock (_userLock)
                {
                    _users.Remove(entry);
                }
            }
        }

        public void RemoveVariable(RoomVariable oldValue)
        {
            lock (_variableLock)
            {
                if (Variables.Contains(oldValue))
                {
                    Variables.Remove(oldValue);
                }
            }
        }
        
        public void AddPlugin(string key, IPlugin plugin)
        {
            _roomPlugins.Add(key, plugin);
        }

        public List<IPlugin> GetRoomPlugins()
        {
            lock (_pluginLock)
            {
                return _roomPlugins.Values.ToList();
            }
        }

        public IPlugin GetPluginByKey(string key)
        {
            lock (_pluginLock)
            {
                if (_roomPlugins.ContainsKey(key))
                {
                    return _roomPlugins[key];
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
