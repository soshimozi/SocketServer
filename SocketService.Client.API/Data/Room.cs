using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SocketService.Framework.SharedObjects;

namespace SocketService.Client.API.Data
{
    public class Room
    {
        private readonly List<User> _userList = new List<User>();
        private readonly Dictionary<String, RoomVariable> _roomVariableList = new Dictionary<String, RoomVariable>();

        private static int _nextId = 0;

        public Room()
        {
            RoomId = NextId();
        }

        private static int NextId()
        {
            return _nextId++;
        }

        public int RoomId
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public int Capacity
        {
            get;
            set;
        }

        public bool IsProtected
        {
            get;
            set;
        }

        public bool IsHidden
        {
            get;
            set;
        }
    
        public void AddUser(User user)
        {
            User u = UserByName(user.UserName);

            if (u == null)
            {
                Monitor.Enter(this);
                try
                {
                    _userList.Add(user);
                }
                finally
                {
                    Monitor.Exit(this);
                }
            }
        }

        public void RemoveUser(string userName)
        {
            User user = UserByName(userName);
            if (user != null)
            {
                Monitor.Enter(this);
                try
                {
                    _userList.Remove(user);
                }
                finally
                {
                    Monitor.Exit(this);
                }
            }
        }

        public User UserByName(string userName)
        {
            Monitor.Enter(this);
            try
            {
                var query = from u in _userList
                            where u.UserName.Equals(userName)
                            select u;

                return query.FirstOrDefault();
            }
            finally
            {
                Monitor.Exit(this);
            }

        }

        public void AddRoomVariable(String Name, RoomVariable Value)
        {
            Monitor.Enter(this);
            try
            {
                _roomVariableList.Add(Name, Value);
            }
            finally
            {
                Monitor.Exit(this);
            }
        }

        public List<User> Users 
        {
            get
            {
                Monitor.Enter(this);
                try
                {
                    return _userList.ToList();
                }
                finally
                {
                    Monitor.Exit(this);
                }
            }
        }

        public void RemoveVariable(string Name)
        {
            Monitor.Enter(this);
            try
            {
                if( _roomVariableList.ContainsKey(Name))
                    _roomVariableList.Remove(Name);
            }
            finally
            {
                Monitor.Exit(this);
            }

        }

        public void UpdateVariable(string Name, RoomVariable Value)
        {
            Monitor.Enter(this);
            try
            {
                if (_roomVariableList.ContainsKey(Name))
                    _roomVariableList.Remove(Name);

                _roomVariableList.Add(Name, Value);
            }
            finally
            {
                Monitor.Exit(this);
            }
        }
    }
}
