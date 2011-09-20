using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SocketService.Client.API.Data;

namespace SocketService.Client.API.Manager
{
    public class UserManager
    {
        private readonly List<User> _userList = new List<User>();

        public User Me
        {
            get;
            set;
        }

        public User AddUser(User user)
        {
            User u = FindByName(user.UserName);
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
            else
            {
                return user = u;
            }
        
            return user;
        }

        public User FindByName(string name)
        {
            Monitor.Enter(this);
            try
            {
                var query = from u in _userList
                            where u.UserName.Equals(name)
                            select u;

                return query.FirstOrDefault();
            }
            finally
            {
                Monitor.Exit(this);
            }
        }



        public void RemoveUser(string userName)
        {
            User user = FindByName(userName);
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
    }
}
