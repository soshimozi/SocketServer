using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.Util;
using SocketService.Framework.SharedObjects;
using SocketService.Framework.Data;

namespace SocketService.Actions
{
    class RoomActionEngine : SingletonBase<RoomActionEngine>
    {
        public Room CreateRoom(string roomName)
        {
            Room room = RoomRepository.Instance.FindByName(roomName);
            if (room == null)
            {
                room = new Room() { Name = roomName };
                RoomRepository.Instance.AddRoom(room);
            }

            return room;
        }


        public void CreateRoomVariable(string roomName, string name, RoomVariable so)
        {
            Room room = RoomRepository.Instance.FindByName(roomName);
            if (room != null)
            {
                room.AddVariable(name, so);
            }
        }

        public RoomVariable GetRoomVariable(string roomName, string variableName)
        {
            RoomVariable so = null;

            Room room = RoomRepository.Instance.FindByName(roomName);
            if (room != null)
            {
                so = room.GetVariable(variableName);
            }

            return so;


        }
    }
}
