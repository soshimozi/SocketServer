using System;

namespace SocketServer.Shared.Request
{
    [Serializable]
    public class CreateRoomRequest
    {
        public string ZoneName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the room.
        /// </summary>
        /// <value>
        /// The name of the room.
        /// </value>
        public string RoomName
        {
            get;
            set;
        }
    }
}
