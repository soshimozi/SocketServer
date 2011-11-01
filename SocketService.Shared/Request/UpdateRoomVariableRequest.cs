using System;

namespace SocketService.Shared.Request
{
    [Serializable]
    public class UpdateRoomVariableRequest
    {
        public int ZoneId { get; set; }

        /// <summary>
        /// Gets or sets the room.
        /// </summary>
        /// <value>
        /// The room.
        /// </value>
        public int RoomId { get; set; }

        /// <summary>
        /// Gets or sets the name of the variable.
        /// </summary>
        /// <value>
        /// The name of the variable.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public object Value { get; set; }
    }
}