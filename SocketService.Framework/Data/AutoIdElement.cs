using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Framework.Data
{
    public class AutoIdElement
    {
        private static object lockObject = new object();

        private static long nextID = -1;

        public static long GetNextID()
        {
            lock (lockObject)
            {
                if (nextID == -1) nextID = DateTime.UtcNow.Ticks; else nextID++;
                return nextID;
            }
        }
    }
}
