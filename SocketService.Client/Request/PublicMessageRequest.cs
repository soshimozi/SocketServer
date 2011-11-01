using System;

namespace SocketService.Client.Core.Request
{
    [Serializable]
    public class PublicMessageRequest
    {
        public int ZoneId { get; set; }

        public int RoomId { get; set; }

        public string User { get; set; }

        public string Message { get; set; }
    }
}