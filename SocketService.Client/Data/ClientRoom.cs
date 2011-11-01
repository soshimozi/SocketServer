using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SocketService.Client.Data
{
    public class ClientRoom
    {
        private readonly List<ClientUser> _userList = new List<ClientUser>();
        private readonly Dictionary<String, object> _roomVariableList = new Dictionary<String, object>();

        private static long _nextId;

        public ClientRoom(long id)
        {
            RoomId = id;
        }

        public ClientRoom()
            : this(NextId())
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
    
        public void AddUser(ClientUser clientUser)
        {
            //User u = UserByName(user.UserName);

            //if (u == null)
            //{
                Monitor.Enter(this);
                try
                {
                    _userList.Add(clientUser);
                }
                finally
                {
                    Monitor.Exit(this);
                }
            //}
        }

        public void RemoveUser(ClientUser clientUser)
        {
            //User user = UserByName(userName);
            //if (user != null)
            //{
                Monitor.Enter(this);
                try
                {
                    _userList.Remove(clientUser);
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

        public void AddRoomVariable(String name, object value)
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

        public List<ClientUser> Users 
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

        public void UpdateVariable(string name, object value)
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
