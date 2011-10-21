using System;
using SocketService.Framework.Client.SharedObjects;

namespace SocketService.Framework.Client.Response
{
    [Serializable]
    public class GetRoomVariableResponse : IResponse
    {
        public long ZoneId { get; set; }

        public long RoomId { get; set; }

        public string Name { get; set; }

        public SharedObject Value { get; set; }
    }
}