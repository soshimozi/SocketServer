using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceHandlerLib;
using BlazeRequestResponse;
using System.ComponentModel.Composition;
using BlazeServerData;
using BlazeSharedObjects;

namespace ServiceHandlers
{
    [Export(typeof(IServiceHandler))]
    [ServiceHandlerType(typeof(GetZonesRequest))]
    class GetZonesRequestHandler : RequestHandlerBase
    {
        private ZoneManager _zoneManager;

        public GetZonesRequestHandler()
        {
            _zoneManager = new ZoneManager(DataEntityFactory.GetDataEntities());
        }

        override public bool HandleRequest(IRequest request, IServerContext server)
        {
            GetZonesRequest zonesRequest = request as GetZonesRequest;
            if (zonesRequest != null)
            {
                GetZonesResponse response = GetZones();
                server.SendObject(response, request.ClientId);
                return true;
            }

            return false;
        }

        GetZonesResponse GetZones()
        {

            IEnumerable<Zone> zones = null;

            try
            {
                zones = _zoneManager.All();
            }
            catch (Exception ex)
            {
                // TODO: Log here
            }

            GetZonesResponse response = new GetZonesResponse();
            if (zones != null)
            {
                var transformQuery = from z in zones
                                     select new ZoneListEntry { ZoneId = z.ZoneId, ZoneName = z.Name };

                response.Zones = transformQuery.ToArray();
            }
            else
            {
                response.Zones = new ZoneListEntry[0];
            }

            return response;
        }
    }
}
