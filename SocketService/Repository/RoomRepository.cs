using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.Data;
using System.Linq.Expressions;

namespace SocketService.Repository
{
    public class RoomRepository : IRepository<Room>, IDisposable
    {
        private static RoomRepository _instance = new RoomRepository();

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static RoomRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RoomRepository();
                }

                return _instance;
            }
        }

        private readonly ServerDataEntities _context;
        public RoomRepository() : this(DatabaseContextFactory.Context)
        {
        }

        public RoomRepository(ServerDataEntities Context)
        {
            _context = Context;
        }

        ///// <summary>
        ///// Adds the room.
        ///// </summary>
        ///// <param name="room">The room.</param>
        //public void AddRoom(Room room)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// Finds the name of the by.
        ///// </summary>
        ///// <param name="name">The name.</param>
        ///// <returns></returns>
        //public Room FindByName(string name)
        //{
        //    throw new NotImplementedException();
        //}


        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
            }

            GC.SuppressFinalize(this);
        }


        public Room Find(int id)
        {
            return _context.Rooms.
                Where(r => r.Id == id).
                FirstOrDefault();
        }

        public Room[] GetAll()
        {
            return _context.Rooms.ToArray();
        }

        public IQueryable<Room> Query(Expression<Func<Room, bool>> filter)
        {
            return _context.Rooms.Where(filter);
        }

        public void Add(Room value)
        {
            _context.Rooms.AddObject(value);
            _context.SaveChanges();
        }

        public void Delete(Room value)
        {
            _context.Rooms.DeleteObject(value);
            _context.SaveChanges();
        }

        public void Update(Room value)
        {
            _context.SaveChanges();
        }
    }
}
