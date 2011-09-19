using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SocketService.Framework.SharedObjects;

namespace SocketService.Client.API
{
    public class Room
    {
        private readonly List<User> _userList = new List<User>();
        private readonly List<RoomVariable> _roomVariableList = new List<RoomVariable>();

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

        public void AddRoomVariable(RoomVariable roomVariable)
        {
            Monitor.Enter(this);
            try
            {
                _roomVariableList.Add(roomVariable);
            }
            finally
            {
                Monitor.Exit(this);
            }
        }
    }
}
