using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Data;
using SocketService.Data.Domain;
using SocketService.Util;
using SocketService.SharedObjects;

namespace SocketService.Actions
{
    class RoomActionEngine : SingletonBase<RoomActionEngine>
    {
        public void CreateRoom(string roomName)
        {
            Room room = RoomRepository.Instance.FindByName(roomName);
            if (room == null)
            {
                RoomRepository.Instance.AddRoom(new Room() { Name = roomName });
            }
        }


        public void CreateRoomVariable(string roomName, string name, ServerObject so)
        {
            Room room = RoomRepository.Instance.FindByName(roomName);
            if (room != null)
            {
                room.AddVariable(name, so);
            }
        }

        public ServerObject GetRoomVariable(string roomName, string variableName)
        {
            ServerObject so = null;

            Room room = RoomRepository.Instance.FindByName(roomName);
            if (room != null)
            {
                so = room.GetVariable(variableName);
            }

            return so;


        }
    }
}
