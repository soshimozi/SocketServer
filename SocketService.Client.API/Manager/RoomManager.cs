using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SocketService.Client.API.Data;
using SocketService.Framework.SharedObjects;

namespace SocketService.Client.API.Manager
{
    public class RoomManager
    {
        private readonly List<Room> _roomList = new List<Room>();

        public Room FindById(long roomId)
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

        public void AddRoomVariable(long roomId, string name, SharedObject value)
        {
            var room = FindById(roomId);
            if (room == null) return;
            room.AddRoomVariable(name, value);
        }

        public void DeleteRoomVariable(long roomId, string name)
        {
            var room = FindById(roomId);
            if (room == null) return;
            room.RemoveVariable(name);
        }

        public void UpdateRoomVariable(long roomId, string name, SharedObject value)
        {
            var room = FindById(roomId);
            if (room == null) return;
            room.UpdateVariable(name, value);
        }

        public void RemoveRoom(long id)
        {
            var room = FindById(id);
            if (room == null) return;
            lock (_roomList)
            {
                _roomList.Remove(room);
            }
        }
    }
}
