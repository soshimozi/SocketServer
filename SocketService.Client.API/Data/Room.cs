using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SocketService.Framework.Client.SharedObjects;

namespace SocketService.Client.API.Data
{
    public class Room
    {
        private readonly List<User> _userList = new List<User>();
        private readonly Dictionary<String, SharedObject> _roomVariableList = new Dictionary<String, SharedObject>();

        private static long _nextId;

        public Room(long id)
        {
            RoomId = id;
        }

        public Room() : this(NextId())
        {
        }

        private static long NextId()
        {
            return _nextId++;
        }

        public long RoomId
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
            //User u = UserByName(user.UserName);

            //if (u == null)
            //{
                Monitor.Enter(this);
                try
                {
                    _userList.Add(user);
                }
                finally
                {
                    Monitor.Exit(this);
                }
            //}
        }

        public void RemoveUser(User user)
        {
            //User user = UserByName(userName);
            //if (user != null)
            //{
                Monitor.Enter(this);
                try
                {
                    _userList.Remove(user);
                }
                finally
                {
                    Monitor.Exit(this);
                }
            //}
        }

        //public User UserByName(string userName)
        //{
        //    Monitor.Enter(this);
        //    try
        //    {
        //        var query = from u in _userList
        //                    where u.UserName.Equals(userName)
        //                    select u;

        //        return query.FirstOrDefault();
        //    }
        //    finally
        //    {
        //        Monitor.Exit(this);
        //    }

        //}

        public void AddRoomVariable(String name, SharedObject value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            Monitor.Enter(this);
            try
            {
                _roomVariableList.Add(name, value);
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

        public void RemoveVariable(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            Monitor.Enter(this);
            try
            {
                if( _roomVariableList.ContainsKey(name))
                    _roomVariableList.Remove(name);
            }
            finally
            {
                Monitor.Exit(this);
            }
        }

        public void UpdateVariable(string name, SharedObject value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (name == null)
                throw new ArgumentNullException("name");

            Monitor.Enter(this);
            try
            {
                if (_roomVariableList.ContainsKey(name))
                    _roomVariableList.Remove(name);

                _roomVariableList.Add(name, value);
            }
            finally
            {
                Monitor.Exit(this);
            }
        }
    }
}
