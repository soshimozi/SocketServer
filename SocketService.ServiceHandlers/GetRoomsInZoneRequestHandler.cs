using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceHandlerLib;
using BlazeRequestResponse;
using BlazeServerData;
using BlazeSharedObjects;
using System.ComponentModel.Composition;

namespace ServiceHandlers
{
    [Export(typeof(IServiceHandler))]
    [ServiceHandlerType(typeof(GetRoomsInZoneRequest))]
    public class GetRoomsInZoneRequestHandler : RequestHandlerBase
    {
        private ZoneManager _zoneManager;

        public GetRoomsInZoneRequestHandler()
        {
            _zoneManager = new ZoneManager(DataEntityFactory.GetDataEntities());
        }

        override public bool HandleRequest(IRequest request, IServerContext server)
        {
            GetRoomsInZoneRequest zonesRequest = request as GetRoomsInZoneRequest;
            if (zonesRequest != null)
            {
                GetRoomsInZoneResponse response = GetRoomsInZone(zonesRequest.ZoneId);
                server.SendObject(response, request.ClientId);
                return true;
            }

            return false;
        }

        private GetRoomsInZoneResponse GetRoomsInZone(int zoneId)
        {
            GetRoomsInZoneResponse response = new GetRoomsInZoneResponse();
            response.ZoneId = zoneId;

            Zone zone = _zoneManager.Search((z) => z.ZoneId == zoneId);
            if (zone != null)
            {
                var transformQuery = from r in zone.Rooms
                                     select new RoomListEntry { RoomId = r.RoomId, RoomName = r.Name };

                response.Rooms = transformQuery.ToArray();                
            }

            return response;
        }
    }
}
