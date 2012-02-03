using System.Linq;
using SocketServer.Core.Data;
using SocketServer.Core.Util;
using SocketServer.Repository;

namespace SocketServer.Actions
{
    class ZoneActionEngine : SingletonBase<ZoneActionEngine>
    {
        public const string DefaultZone = "";

        public Zone CreateZone(string zoneName)
        {
            var zone = ZoneRepository.Instance.Query( z => z.Name.Equals(zoneName) ).FirstOrDefault();
            if (zone == null)
            {
                zone = new Zone { Name = zoneName };
                ZoneRepository.Instance.Add(zone);
            }

            return zone;
        }

    }
}
