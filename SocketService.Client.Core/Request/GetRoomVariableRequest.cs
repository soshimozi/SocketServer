using System;

namespace SocketService.Client.Core.Request
{
    [Serializable]
    public class GetRoomVariableRequest
    {
        public int ZoneId { get; set; }

        /// <summary>
        /// Gets or sets the name of the room.
        /// </summary>
        /// <value>
        /// The name of the room.
        /// </value>
        public int RoomId { get; set; }

        /// <summary>
        /// Gets or sets the name of the variable.
        /// </summary>
        /// <value>
        /// The name of the variable.
        /// </value>
        public string VariableName { get; set; }
    }
}