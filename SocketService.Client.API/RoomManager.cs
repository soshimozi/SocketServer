using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SocketService.Client.API
{
    public class RoomManager
    {
        private readonly List<Room> _roomList = new List<Room>();
        public RoomManager()
        {
        }

        public Room FindById(int roomId)
        {
            Monitor.Enter(this);
            try
            {
                var query = from r in _roomList
                            where r.RoomId.Equals(roomId)
                            select r;

                return query.FirstOrDefault();
            }
            finally
            {
                Monitor.Exit(this);
            }
        }

        public void AddRoom(Room room)
        {
            Monitor.Enter(this);
            try
            {
                _roomList.Add(room);
            }
            finally
            {
                Monitor.Exit(this);
            }
        }
    }
}
