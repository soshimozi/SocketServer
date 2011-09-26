using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.Util;
using SocketService.Framework.SharedObjects;
using SocketService.Framework.Data;
using SocketService.Repository;

namespace SocketService.Actions
{
    class RoomActionEngine : SingletonBase<RoomActionEngine>
    {
        public const string DefaultRoom = "";

        public Room CreateRoom(string roomName)
        {
            Room room = RoomRepository.Instance.Query( r => r.Name.Equals(roomName) ).FirstOrDefault();
            if (room == null)
            {
                room = new Room() { Name = roomName };
                RoomRepository.Instance.Add(room);
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

        public RoomVariable GetRoomVariable(string roomName, string variableName)
        {
            //RoomVariable so = null;

            //Room room = RoomRepository.Instance.FindByName(roomName);
            //if (room != null)
            //{
            //    so = room.GetVariable(variableName);
            //}

            //return so;

            throw new NotImplementedException();


        }

        public void UpdateRoomVariable(int RoomId, string Name, RoomVariable Value)
        {
            //Room room = RoomRepository.Instance.Find(RoomId);
            //if (room != null)
            //{
            //    RoomVariable oldValue = room.GetVariable(Name);
            //    if (oldValue != null)
            //    {
            //        room.RemoveVariable(oldValue);
            //    }

            //    room.AddVariable(Name, Value);
            //}
            throw new NotImplementedException();

        }

        public void DeleteRoomVariable(int RoomId, string Name)
        {
            //Room room = RoomRepository.Instance.Find(RoomId);
            //if (room != null)
            //{
            //    RoomVariable oldValue = room.GetVariable(Name);
            //    if (oldValue != null)
            //    {
            //        room.RemoveVariable(oldValue);
            //    }
            //}
            throw new NotImplementedException();

        }

        public Room GetRoomByName(string name)
        {
            //return RoomRepository.Instance.FindByName(name);
            throw new NotImplementedException();
        }

        public Room GetRoomById(int roomId)
        {
            throw new NotImplementedException();
        }
    }
}
