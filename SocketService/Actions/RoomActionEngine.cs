using System.Linq;
using SocketServer.Data;
using SocketServer.Repository;
using RoomVariable = SocketServer.Data.RoomVariable;
using Zone = SocketServer.Data.Zone;

namespace SocketServer.Actions
{
    class RoomActionEngine : SingletonBase<RoomActionEngine>
    {
        public const string DefaultRoom = "";

        public Room CreateRoom(string roomName, Zone zone)
        {
            var room = RoomRepository.Instance.Query( r => r.Name.Equals(roomName) ).FirstOrDefault();
            if (room == null)
            {
                room = new Room { Name = roomName, Password = string.Empty, Capacity = -1, IsPersistable = false, IsPrivate = false, Zone = zone };

                RoomRepository.Instance.Add(room);
                zone.Rooms.Add(room);

                ZoneRepository.Instance.Update(zone);
                RoomRepository.Instance.Update(room);
            }

            return room;
        }


        //public void CreateRoomVariable(string roomName, string name, RoomVariable so)
        //{
        //    Room room = RoomRepository.Instance.FindByName(roomName);
        //    if (room != null)
        //    {
        //        room.AddVariable(name, so);
        //    }
        //}

        //public RoomVariable GetRoomVariable(string roomName, string variableName)
        //{
        //    //RoomVariable so = null;

        //    //Room room = RoomRepository.Instance.FindByName(roomName);
        //    //if (room != null)
        //    //{
        //    //    so = room.GetVariable(variableName);
        //    //}

        //    //return so;

        //    throw new NotImplementedException();


        //}

        //public void UpdateRoomVariable(int RoomId, string Name, RoomVariable Value)
        //{
        //    //Room room = RoomRepository.Instance.Find(RoomId);
        //    //if (room != null)
        //    //{
        //    //    RoomVariable oldValue = room.GetVariable(Name);
        //    //    if (oldValue != null)
        //    //    {
        //    //        room.RemoveVariable(oldValue);
        //    //    }

        //    //    room.AddVariable(Name, Value);
        //    //}
        //    throw new NotImplementedException();

        //}

        //public void DeleteRoomVariable(int RoomId, string Name)
        //{
        //    //Room room = RoomRepository.Instance.Find(RoomId);
        //    //if (room != null)
        //    //{
        //    //    RoomVariable oldValue = room.GetVariable(Name);
        //    //    if (oldValue != null)
        //    //    {
        //    //        room.RemoveVariable(oldValue);
        //    //    }
        //    //}
        //    throw new NotImplementedException();

        //}

        //public Room GetRoomByName(string name)
        //{
        //    //return RoomRepository.Instance.FindByName(name);
        //    throw new NotImplementedException();
        //}

        //public Room GetRoomById(int roomId)
        //{
        //    throw new NotImplementedException();
        //}
        
        public void CreateRoomVariable(Room room, string variableName, byte [] value)
        {
            var variable = new RoomVariable();
            variable.Name = variableName;
            variable.Value = value;

            room.RoomVariables.Add(variable);
            RoomRepository.Instance.Update(room);            
        }

        public void RemoveNonPersistentRooms()
        {
            var rooms = RoomRepository.Instance.Query(r => !r.IsPersistable).ToList();
            foreach (var room in rooms)
            {
                var zone = room.Zone;
                if (zone != null)
                {
                    zone.Rooms.Remove(room);

                    RoomRepository.Instance.Delete(room);
                    ZoneRepository.Instance.Update(zone);
                }
                else
                {
                    RoomRepository.Instance.Delete(room);
                }
            }
        }
    }
}
