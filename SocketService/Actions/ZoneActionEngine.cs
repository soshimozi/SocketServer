using System.Linq;
using SocketService.Core.Data;
using SocketService.Core.Util;
using SocketService.Framework.Data;
using SocketService.Repository;

namespace SocketService.Actions
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
