using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Framework.Data
{
    class Zone
    {
        public Zone()
        {
            Id = NextId();
        }

        private static int nextId = 0;
        protected static int NextId()
        {
            return nextId++;
        }

        public int Id
        {
            get;
            private set;
        }
    }
}
