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

        public ServerObject Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public int ZoneId
        {
            get { return zoneId; }
            set { zoneId = value; }
        }

        public int RoomId
        {
            get { return roomId; }
            set { roomId = value; }
        }


        public bool Locked
        {
            get { return locked; }
            set { locked = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public bool Persistent
        {
            get { return persistent; }
            set { persistent = value; }
        }

    }
}
