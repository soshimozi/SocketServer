using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SocketService.Client.API.Data;
using SocketService.Framework.SharedObjects;

namespace SocketService.Client.API.Manager
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

        public void AddRoomVariable(int RoomId, string Name, SharedObject Value)
        {
            Room room = FindById(RoomId);
            if (room != null)
            {
                room.AddRoomVariable(Name, Value);
            }
        }

        public void DeleteRoomVariable(int RoomId, string Name)
        {
            Room room = FindById(RoomId);
            if ( room != null )
            {
                room.RemoveVariable(Name);
            }
        }

        public void UpdateRoomVariable(int RoomId, string Name, SharedObject Value)
        {
            Room room = FindById(RoomId);
            if (room != null)
            {
                room.UpdateVariable(Name, Value);
            }
        }

        public void RemoveRoom(int Id)
        {
            Room room = FindById(Id);
            if (room != null)
            {
                lock (_roomList)
                {
                    _roomList.Remove(room);
                }
            }
        }
    }
}
