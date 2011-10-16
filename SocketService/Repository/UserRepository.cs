using System;
using System.Linq;
using SocketService.Framework.Data;

namespace SocketService.Repository
{
    public class UserRepository : IRepository<User>, IDisposable
    {
        private static UserRepository _instance;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static UserRepository Instance
        {
            get { return _instance ?? (_instance = new UserRepository()); }
        }

        private readonly ServerDataEntities _context;

        #region ctor
        public UserRepository() : this(DatabaseContextFactory.Context)
        {
        }

        public UserRepository(ServerDataEntities context)
        {
            if (context == null) throw new ArgumentNullException("context");
            _context = context;
        }

        #endregion
        ///// <summary>
        ///// Finds the users by room.
        ///// </summary>
        ///// <param name="roomname">The roomname.</param>
        ///// <returns></returns>
        //public List<User> FindUsersByRoom(string roomname)
        //{
        //    throw new NotImplementedException();

        //}

        //public List<Guid> FindClientKeysByRoomFiltered(string roomname, Guid filteredClient)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// Finds the name of the user by.
        ///// </summary>
        ///// <param name="username">The username.</param>
        ///// <returns></returns>
        //public User FindByName(string username)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// Finds the user by client key.
        ///// </summary>
        ///// <param name="clientKey">The client key.</param>
        ///// <returns></returns>
        //public User FindUserByClientKey(Guid clientKey)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// Adds the user.
        ///// </summary>
        ///// <param name="user">The user.</param>
        //public void AddUser(User user)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// Removes the user.
        ///// </summary>
        ///// <param name="user">The user.</param>
        //public void RemoveUser(User user)
        //{
        //    throw new NotImplementedException();
        //}

        //public List<Guid> FindClientKeysByRoom(string RoomName)
        //{
        //    throw new NotImplementedException();
        //}

        #region IRepository<User> Members
        public User Find(int id)
        {
            return _context.Users.
                Where(u => u.Id == id).
                FirstOrDefault();
        }

        public User[] GetAll()
        {
            return _context.Users.ToArray();
        }

        public IQueryable<User> Query(System.Linq.Expressions.Expression<Func<User, bool>> filter)
        {
            return _context.Users.Where(filter);
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
            }

            GC.SuppressFinalize(this);
        }

        #endregion


        public void Add(User value)
        {
            _context.AddToUsers(value);
            _context.SaveChanges();
        }

        public void Delete(User value)
        {
            _context.DeleteObject(value);
            _context.SaveChanges();
        }

        public void Update(User value)
        {
            _context.SaveChanges();
        }
    }
}
