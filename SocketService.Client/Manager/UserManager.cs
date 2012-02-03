using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SocketServer.Client.Data;

namespace SocketServer.Client.Manager
{
    public class UserManager
    {
        private readonly List<ClientUser> _userList = new List<ClientUser>();

        public ClientUser Me
        {
            get;
            set;
        }

        public ClientUser AddUser(ClientUser user)
        {
            var u = FindByName(user.UserName);

            // didn't find it, so enter the user
            if (u != null) return u;
            
            Monitor.Enter(this);
            try
            {
                _userList.Add(user);
            }
            finally
            {
                Monitor.Exit(this);
            }

            return user;
        }

        public ClientUser FindByName(string name)
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
            var user = FindByName(userName);
            if (user == null) return;

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
