using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.Data;

namespace SocketService
{
    public class RoomRepository
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
        /// <summary>
        /// Adds the room.
        /// </summary>
        /// <param name="room">The room.</param>
        public void AddRoom(Room room)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds the name of the by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public Room FindByName(string name)
        {
            throw new NotImplementedException();
        }

        public Room Find(int RoomId)
        {
            throw new NotImplementedException();
        }
    }
}
