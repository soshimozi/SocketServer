using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Framework.SharedObjects
{
    [Serializable]
    public class RoomVariable
    {
        private bool persistent;
        private string name;
        private bool locked;
        private int roomId;
        private int zoneId;
        private ServerObject value;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public ServerObject Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        /// <summary>
        /// Gets or sets the zone id.
        /// </summary>
        /// <value>
        /// The zone id.
        /// </value>
        public int ZoneId
        {
            get { return zoneId; }
            set { zoneId = value; }
        }

        /// <summary>
        /// Gets or sets the room id.
        /// </summary>
        /// <value>
        /// The room id.
        /// </value>
        public int RoomId
        {
            get { return roomId; }
            set { roomId = value; }
        }


        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RoomVariable"/> is locked.
        /// </summary>
        /// <value>
        ///   <c>true</c> if locked; otherwise, <c>false</c>.
        /// </value>
        public bool Locked
        {
            get { return locked; }
            set { locked = value; }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RoomVariable"/> is persistent.
        /// </summary>
        /// <value>
        ///   <c>true</c> if persistent; otherwise, <c>false</c>.
        /// </value>
        public bool Persistent
        {
            get { return persistent; }
            set { persistent = value; }
        }

    }
}
