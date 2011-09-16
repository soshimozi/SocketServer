using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using ServiceHandlerLib;
using BlazeRequestResponse;
using BlazeServerData;
using BlazeSharedObjects;

namespace ServiceHandlers
{
    [Export(typeof(IServiceHandler))]
    [ServiceHandlerType(typeof(CreateRoomRequest))]
    class CreateRoomRequestHandler : IServiceHandler
    {
        private UserManager _userManager;
        private RoomManager _roomManager;
        private ZoneManager _zoneManager;

        public CreateRoomRequestHandler()
        {
            _userManager = new UserManager(DataEntityFactory.GetDataEntities());
            _roomManager = new RoomManager(DataEntityFactory.GetDataEntities());
            _zoneManager = new ZoneManager(DataEntityFactory.GetDataEntities());
        }

        #region IServiceHandler Members

        public bool HandleRequest(IRequest request, IServerContext server)
        {
            CreateRoomRequest crRequest = request as CreateRoomRequest;

            if (crRequest != null)
            {
                CreateRoom(
                        server, 
                        crRequest.ClientId, 
                        crRequest.RequestId, 
                        crRequest.RoomName, 
                        crRequest.ZoneName,
                        crRequest.JoinRoom);

                //server.SendResponse(response, crRequest.ClientId);
                return true;
            }

            return false;
        }

        public void CreateRoom(IServerContext server, Guid clientId, string requestId, string roomName, string zoneName, bool joinRoom)
        {
            //IResponse response = null;

            bool newZone = false;

            Zone zone = _zoneManager.FindZone(zoneName);
            if (zone == null)
            {
                // create the zone
                zone = _zoneManager.CreateZone();
                zone.Name = zoneName;

                newZone = true;
            }

            // see if this room is already in the zone
            Room room = _zoneManager.FindRoomInZone(zone, roomName);
            if (room == null)
            {
                room = _roomManager.FindRoom(roomName);

                // if room is still not found
                if (room == null)
                {
                    room = _roomManager.CreateRoom();
                    room.Name = roomName;
                    room.Zone = zone;
                    _roomManager.SaveChanges();
                }
            }


            //response = new CreateRoomResponse(true, requestId, room.RoomId, zone.ZoneId);

            int? userId = server.GetUserIdForClientId(clientId);
            if (userId.HasValue)
            {
                User user = _userManager.Search((u) => { return u.UserId == userId; });

                if (user != null && joinRoom)
                {
                    user.Room = room;

                    _userManager.SaveChanges();

                    JoinRoomEvent joinRoomEvent = new JoinRoomEvent();
                    joinRoomEvent.RoomId = room.RoomId;
                    joinRoomEvent.ZoneId = room.ZoneId;

                    var transformQuery = from userRecord in room.Users
                                         select new UserListEntry { UserId = userRecord.UserId, UserName = userRecord.UserName };

                    joinRoomEvent.Users = transformQuery.ToArray();

                    server.SendObject(joinRoomEvent, clientId);
                }
            }

            // broadcast to everyone that we created a new room
            ZoneUpdateEvent evt = new ZoneUpdateEvent();
            evt.RoomId = room.RoomId;
            evt.ZoneId = zone.ZoneId;
            evt.RoomCount = zone.Rooms.Count;

            // only send name on new zone for creation on client side
            if (newZone)
            {
                evt.ZoneName = zone.Name;
            }

            // action is create or update only, not delete should occur in this function
            evt.Action = (newZone ? 0 : 1);
            server.BroadcastObject(evt);

            //return response;
        }
        #endregion
    }
}
